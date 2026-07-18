using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Data;
using AttendanceSystem.Data.Entities;
using AttendanceSystem.Data.Entities.Enums;

namespace AttendanceSystem.Business.Services
{
    public class ClassSubjectService : IClassSubjectService
    {
        private readonly AppDbContext _context;

        public ClassSubjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassSubjectDto>> GetAllAsync(int? semesterId = null)
        {
            var query = _context.ClassSubjects
                .Include(cs => cs.Class)
                .Include(cs => cs.Subject)
                .Include(cs => cs.Lecturer).ThenInclude(l => l.User)
                .Include(cs => cs.Semester)
                .Where(cs => !cs.IsDeleted);

            if (semesterId.HasValue && semesterId.Value > 0)
            {
                query = query.Where(cs => cs.SemesterId == semesterId.Value);
            }

            return await query
                .Select(cs => new ClassSubjectDto
                {
                    Id = cs.Id,
                    ClassId = cs.ClassId,
                    SubjectId = cs.SubjectId,
                    LecturerId = cs.LecturerId,
                    SemesterId = cs.SemesterId,
                    Status = (int)cs.Status,
                    ClassCode = cs.Class.ClassCode,
                    SubjectCode = cs.Subject.SubjectCode,
                    SubjectName = cs.Subject.SubjectName,
                    LecturerName = cs.Lecturer.User.FullName,
                    SemesterName = cs.Semester.Name
                })
                .ToListAsync();
        }

        public async Task<ClassSubjectDto?> GetByIdAsync(int id)
        {
            var entity = await _context.ClassSubjects
                .Include(cs => cs.Class)
                .Include(cs => cs.Subject)
                .Include(cs => cs.Lecturer).ThenInclude(l => l.User)
                .Include(cs => cs.Semester)
                .FirstOrDefaultAsync(cs => cs.Id == id && !cs.IsDeleted);

            if (entity == null) return null;

            return new ClassSubjectDto
            {
                Id = entity.Id,
                ClassId = entity.ClassId,
                SubjectId = entity.SubjectId,
                LecturerId = entity.LecturerId,
                SemesterId = entity.SemesterId,
                Status = (int)entity.Status,
                ClassCode = entity.Class.ClassCode,
                SubjectCode = entity.Subject.SubjectCode,
                SubjectName = entity.Subject.SubjectName,
                LecturerName = entity.Lecturer.User.FullName,
                SemesterName = entity.Semester.Name
            };
        }

        public async Task<ClassSubjectDto> CreateAsync(ClassSubjectDto dto)
        {
            var exists = await ExistsAsync(dto.ClassId, dto.SubjectId, dto.SemesterId);
            if (exists) throw new InvalidOperationException("Lớp này đã được phân công môn học này trong học kỳ này.");

            var semester = await _context.Semesters.FindAsync(dto.SemesterId);
            var subject = await _context.Subjects.FindAsync(dto.SubjectId);
            if (semester != null && subject != null)
            {
                double weeks = (semester.EndDate - semester.StartDate).TotalDays / 7.0;
                if (weeks > 0 && subject.TotalSlots > weeks * 4)
                {
                    throw new InvalidOperationException($"Học kỳ {semester.Name} quá ngắn ({Math.Round(weeks, 1)} tuần) để hoàn thành môn học {subject.SubjectCode} ({subject.TotalSlots} slots, tối đa {Math.Round(weeks * 4)} slots).");
                }
            }

            var entity = new ClassSubject
            {
                ClassId = dto.ClassId,
                SubjectId = dto.SubjectId,
                LecturerId = dto.LecturerId,
                SemesterId = dto.SemesterId,
                Status = ClassSubjectStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _context.ClassSubjects.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<ClassSubjectDto?> UpdateAsync(int id, ClassSubjectDto dto)
        {
            var exists = await ExistsAsync(dto.ClassId, dto.SubjectId, dto.SemesterId, id);
            if (exists) throw new InvalidOperationException("Lớp này đã được phân công môn học này trong học kỳ này.");

            var semester = await _context.Semesters.FindAsync(dto.SemesterId);
            var subject = await _context.Subjects.FindAsync(dto.SubjectId);
            if (semester != null && subject != null)
            {
                double weeks = (semester.EndDate - semester.StartDate).TotalDays / 7.0;
                if (weeks > 0 && subject.TotalSlots > weeks * 4)
                {
                    throw new InvalidOperationException($"Học kỳ {semester.Name} quá ngắn ({Math.Round(weeks, 1)} tuần) để hoàn thành môn học {subject.SubjectCode} ({subject.TotalSlots} slots, tối đa {Math.Round(weeks * 4)} slots).");
                }
            }

            var entity = await _context.ClassSubjects.FirstOrDefaultAsync(cs => cs.Id == id && !cs.IsDeleted);
            if (entity == null) return null;

            entity.ClassId = dto.ClassId;
            entity.SubjectId = dto.SubjectId;
            entity.LecturerId = dto.LecturerId;
            entity.SemesterId = dto.SemesterId;
            entity.Status = (ClassSubjectStatus)dto.Status;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ClassSubjects.FirstOrDefaultAsync(cs => cs.Id == id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int classId, int subjectId, int semesterId, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.ClassSubjects.AnyAsync(cs => cs.ClassId == classId && cs.SubjectId == subjectId && cs.SemesterId == semesterId && cs.Id != excludeId.Value && !cs.IsDeleted);
            }
            return await _context.ClassSubjects.AnyAsync(cs => cs.ClassId == classId && cs.SubjectId == subjectId && cs.SemesterId == semesterId && !cs.IsDeleted);
        }
    }
}
