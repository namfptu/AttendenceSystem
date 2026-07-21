using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Data;
using AttendanceSystem.Data.Entities;

namespace AttendanceSystem.Business.Services
{
    public class ClassSubstituteService : IClassSubstituteService
    {
        private readonly AppDbContext _context;

        public ClassSubstituteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassSubstituteDto>> GetAllAsync(int? classSubjectId = null)
        {
            var query = _context.ClassSubstitutes
                .Include(cs => cs.ClassSubject).ThenInclude(csub => csub.Class)
                .Include(cs => cs.ClassSubject).ThenInclude(csub => csub.Subject)
                .Include(cs => cs.ClassSubject).ThenInclude(csub => csub.Lecturer).ThenInclude(l => l.User)
                .Include(cs => cs.Lecturer).ThenInclude(l => l.User)
                .Where(cs => !cs.IsDeleted);

            if (classSubjectId.HasValue && classSubjectId.Value > 0)
            {
                query = query.Where(cs => cs.ClassSubjectId == classSubjectId.Value);
            }

            return await query
                .Select(cs => new ClassSubstituteDto
                {
                    Id = cs.Id,
                    ClassSubjectId = cs.ClassSubjectId,
                    LecturerId = cs.LecturerId,
                    SubstituteDate = cs.SubstituteDate,
                    Note = cs.Note,
                    ClassCode = cs.ClassSubject.Class.ClassCode,
                    SubjectCode = cs.ClassSubject.Subject.SubjectCode,
                    SubjectName = cs.ClassSubject.Subject.SubjectName,
                    PrimaryLecturerName = cs.ClassSubject.Lecturer.User.FullName,
                    SubstituteLecturerName = cs.Lecturer.User.FullName
                })
                .ToListAsync();
        }

        public async Task<ClassSubstituteDto?> GetByIdAsync(int id)
        {
            var entity = await _context.ClassSubstitutes
                .Include(cs => cs.ClassSubject).ThenInclude(csub => csub.Class)
                .Include(cs => cs.ClassSubject).ThenInclude(csub => csub.Subject)
                .Include(cs => cs.ClassSubject).ThenInclude(csub => csub.Lecturer).ThenInclude(l => l.User)
                .Include(cs => cs.Lecturer).ThenInclude(l => l.User)
                .FirstOrDefaultAsync(cs => cs.Id == id && !cs.IsDeleted);

            if (entity == null) return null;

            return new ClassSubstituteDto
            {
                Id = entity.Id,
                ClassSubjectId = entity.ClassSubjectId,
                LecturerId = entity.LecturerId,
                SubstituteDate = entity.SubstituteDate,
                Note = entity.Note,
                ClassCode = entity.ClassSubject.Class.ClassCode,
                SubjectCode = entity.ClassSubject.Subject.SubjectCode,
                SubjectName = entity.ClassSubject.Subject.SubjectName,
                PrimaryLecturerName = entity.ClassSubject.Lecturer.User.FullName,
                SubstituteLecturerName = entity.Lecturer.User.FullName
            };
        }

        public async Task<ClassSubstituteDto> CreateAsync(ClassSubstituteDto dto)
        {
            var classSubject = await _context.ClassSubjects
                .Include(cs => cs.Class)
                .Include(cs => cs.Subject)
                .Include(cs => cs.Semester)
                .FirstOrDefaultAsync(cs => cs.Id == dto.ClassSubjectId && !cs.IsDeleted);

            if (classSubject == null)
            {
                throw new InvalidOperationException("Lớp học phần không tồn tại.");
            }

            var lecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Id == dto.LecturerId && !l.IsDeleted);
            if (lecturer == null)
            {
                throw new InvalidOperationException("Giảng viên dạy thay không tồn tại.");
            }

            // 1. Không tự dạy thay chính mình
            if (classSubject.LecturerId == dto.LecturerId)
            {
                throw new InvalidOperationException("Giảng viên được gán dạy thay trùng với Giảng viên chính của lớp học phần.");
            }

            // 2. Ngày dạy thế phải nằm trong thời gian học kỳ
            if (dto.SubstituteDate.Date < classSubject.Semester.StartDate.Date || dto.SubstituteDate.Date > classSubject.Semester.EndDate.Date)
            {
                throw new InvalidOperationException("Ngày dạy thay phải nằm trong khoảng thời gian diễn ra học kỳ.");
            }

            // 3. Đã có người dạy thay cho ngày đó chưa
            var existingSub = await _context.ClassSubstitutes
                .AnyAsync(cs => cs.ClassSubjectId == dto.ClassSubjectId 
                             && cs.SubstituteDate.Date == dto.SubstituteDate.Date 
                             && !cs.IsDeleted);
            if (existingSub)
            {
                throw new InvalidOperationException("Lớp học phần này đã được phân công giảng viên dạy thay khác trong ngày này.");
            }

            // 4. Tìm các tiết học chính thức của ClassSubject trong ngày đó
            var dayOfWeek = dto.SubstituteDate.DayOfWeek;
            var classSchedules = await _context.Schedules
                .Where(s => s.ClassSubjectId == dto.ClassSubjectId && s.DayOfWeek == dayOfWeek && !s.IsDeleted)
                .ToListAsync();

            var hasManualSessions = await _context.AttendanceSessions
                .AnyAsync(s => s.ClassSubjectId == dto.ClassSubjectId && s.SessionDate.Date == dto.SubstituteDate.Date && !s.IsDeleted);

            if (!classSchedules.Any() && !hasManualSessions)
            {
                throw new InvalidOperationException("Ngày được gán dạy thay không trùng với bất kỳ lịch học hoặc phiên học nào của môn này.");
            }

            // 5. Kiểm tra trùng lịch bận của giảng viên dạy thay trong ngày này
            var lecturerSchedules = await _context.Schedules
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Subject)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Class)
                .Where(s => s.ClassSubject.LecturerId == dto.LecturerId 
                         && s.DayOfWeek == dayOfWeek 
                         && !s.IsDeleted 
                         && s.ClassSubject.SemesterId == classSubject.SemesterId)
                .ToListAsync();

            var lecturerSubstitutes = await _context.ClassSubstitutes
                .Where(cs => cs.LecturerId == dto.LecturerId 
                          && cs.SubstituteDate.Date == dto.SubstituteDate.Date 
                          && !cs.IsDeleted)
                .ToListAsync();

            foreach (var cs in classSchedules)
            {
                // Kiểm tra lịch dạy chính thức của giảng viên dạy thay
                var overlapOwn = lecturerSchedules.FirstOrDefault(ls => cs.StartTime < ls.EndTime && cs.EndTime > ls.StartTime);
                if (overlapOwn != null)
                {
                    throw new InvalidOperationException($"Giảng viên dạy thay đã có lịch giảng dạy môn {overlapOwn.ClassSubject.Subject.SubjectCode} của lớp {overlapOwn.ClassSubject.Class.ClassCode} vào khung giờ {overlapOwn.StartTime:hh\\:mm} - {overlapOwn.EndTime:hh\\:mm} trong ngày này.");
                }

                // Kiểm tra lịch đã nhận dạy thay khác của giảng viên này
                foreach (var sub in lecturerSubstitutes)
                {
                    var subSchedules = await _context.Schedules
                        .Include(s => s.ClassSubject).ThenInclude(csub => csub.Subject)
                        .Include(s => s.ClassSubject).ThenInclude(csub => csub.Class)
                        .Where(s => s.ClassSubjectId == sub.ClassSubjectId && s.DayOfWeek == dayOfWeek && !s.IsDeleted)
                        .ToListAsync();

                    var overlapSub = subSchedules.FirstOrDefault(ss => cs.StartTime < ss.EndTime && cs.EndTime > ss.StartTime);
                    if (overlapSub != null)
                    {
                        throw new InvalidOperationException($"Giảng viên dạy thay đã được phân công dạy thế cho lớp {overlapSub.ClassSubject.Class.ClassCode} môn {overlapSub.ClassSubject.Subject.SubjectCode} vào khung giờ {overlapSub.StartTime:hh\\:mm} - {overlapSub.EndTime:hh\\:mm} trong ngày này.");
                    }
                }
            }

            var entity = new ClassSubstitute
            {
                ClassSubjectId = dto.ClassSubjectId,
                LecturerId = dto.LecturerId,
                SubstituteDate = dto.SubstituteDate,
                Note = dto.Note ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _context.ClassSubstitutes.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ClassSubstitutes
                .FirstOrDefaultAsync(cs => cs.Id == id && !cs.IsDeleted);

            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
