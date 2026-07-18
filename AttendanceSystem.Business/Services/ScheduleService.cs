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
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _context;

        public ScheduleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ScheduleDto>> GetAllAsync()
        {
            return await _context.Schedules
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Class)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Subject)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Lecturer).ThenInclude(l => l.User)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Semester)
                .Where(s => !s.IsDeleted)
                .Select(s => new ScheduleDto
                {
                    Id = s.Id,
                    ClassSubjectId = s.ClassSubjectId,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Room = s.Room,
                    ClassName = s.ClassSubject.Class.ClassName,
                    SubjectName = s.ClassSubject.Subject.SubjectName,
                    LecturerName = s.ClassSubject.Lecturer.User.FullName,
                    SemesterName = s.ClassSubject.Semester.Name,
                    ClassSubjectDisplay = s.ClassSubject.Class.ClassCode + " - " + s.ClassSubject.Subject.SubjectName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleDto>> GetByClassSubjectIdAsync(int classSubjectId)
        {
            return await _context.Schedules
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Class)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Subject)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Lecturer).ThenInclude(l => l.User)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Semester)
                .Where(s => s.ClassSubjectId == classSubjectId && !s.IsDeleted)
                .Select(s => new ScheduleDto
                {
                    Id = s.Id,
                    ClassSubjectId = s.ClassSubjectId,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Room = s.Room,
                    ClassName = s.ClassSubject.Class.ClassName,
                    SubjectName = s.ClassSubject.Subject.SubjectName,
                    LecturerName = s.ClassSubject.Lecturer.User.FullName,
                    SemesterName = s.ClassSubject.Semester.Name,
                    ClassSubjectDisplay = s.ClassSubject.Class.ClassCode + " - " + s.ClassSubject.Subject.SubjectName
                })
                .ToListAsync();
        }

        public async Task<ScheduleDto> GetByIdAsync(int id)
        {
            var s = await _context.Schedules
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Class)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Subject)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Lecturer).ThenInclude(l => l.User)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Semester)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (s == null) return null;

            return new ScheduleDto
            {
                Id = s.Id,
                ClassSubjectId = s.ClassSubjectId,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Room = s.Room,
                ClassName = s.ClassSubject.Class.ClassName,
                SubjectName = s.ClassSubject.Subject.SubjectName,
                LecturerName = s.ClassSubject.Lecturer.User.FullName,
                SemesterName = s.ClassSubject.Semester.Name,
                ClassSubjectDisplay = s.ClassSubject.Class.ClassCode + " - " + s.ClassSubject.Subject.SubjectName
            };
        }

        public async Task<ScheduleDto> CreateAsync(ScheduleDto dto)
        {
            var classSubject = await _context.ClassSubjects.FirstOrDefaultAsync(cs => cs.Id == dto.ClassSubjectId);
            if (classSubject != null)
            {
                bool lecturerOverlap = await _context.Schedules.Include(s => s.ClassSubject)
                    .AnyAsync(s => !s.IsDeleted && s.DayOfWeek == dto.DayOfWeek 
                                && s.ClassSubject.LecturerId == classSubject.LecturerId 
                                && s.StartTime < dto.EndTime && s.EndTime > dto.StartTime);
                if (lecturerOverlap) throw new Exception("Giảng viên đã có lịch dạy bị trùng giờ trong ngày này.");

                bool roomOverlap = await _context.Schedules
                    .AnyAsync(s => !s.IsDeleted && s.DayOfWeek == dto.DayOfWeek 
                                && s.Room == dto.Room 
                                && s.StartTime < dto.EndTime && s.EndTime > dto.StartTime);
                if (roomOverlap) throw new Exception("Phòng học đã được xếp lịch trùng giờ trong ngày này.");

                bool classOverlap = await _context.Schedules.Include(s => s.ClassSubject)
                    .AnyAsync(s => !s.IsDeleted && s.DayOfWeek == dto.DayOfWeek 
                                && s.ClassSubject.ClassId == classSubject.ClassId 
                                && s.StartTime < dto.EndTime && s.EndTime > dto.StartTime);
                if (classOverlap) throw new Exception("Lớp học đã có môn học khác bị trùng giờ trong ngày này.");
            }

            var schedule = new Schedule
            {
                ClassSubjectId = dto.ClassSubjectId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Room = dto.Room
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            dto.Id = schedule.Id;
            return dto;
        }

        public async Task<ScheduleDto> UpdateAsync(int id, ScheduleDto dto)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            if (schedule == null) return null;

            var classSubject = await _context.ClassSubjects.FirstOrDefaultAsync(cs => cs.Id == dto.ClassSubjectId);
            if (classSubject != null)
            {
                bool lecturerOverlap = await _context.Schedules.Include(s => s.ClassSubject)
                    .AnyAsync(s => !s.IsDeleted && s.Id != id && s.DayOfWeek == dto.DayOfWeek 
                                && s.ClassSubject.LecturerId == classSubject.LecturerId 
                                && s.StartTime < dto.EndTime && s.EndTime > dto.StartTime);
                if (lecturerOverlap) throw new Exception("Giảng viên đã có lịch dạy bị trùng giờ trong ngày này.");

                bool roomOverlap = await _context.Schedules
                    .AnyAsync(s => !s.IsDeleted && s.Id != id && s.DayOfWeek == dto.DayOfWeek 
                                && s.Room == dto.Room 
                                && s.StartTime < dto.EndTime && s.EndTime > dto.StartTime);
                if (roomOverlap) throw new Exception("Phòng học đã được xếp lịch trùng giờ trong ngày này.");

                bool classOverlap = await _context.Schedules.Include(s => s.ClassSubject)
                    .AnyAsync(s => !s.IsDeleted && s.Id != id && s.DayOfWeek == dto.DayOfWeek 
                                && s.ClassSubject.ClassId == classSubject.ClassId 
                                && s.StartTime < dto.EndTime && s.EndTime > dto.StartTime);
                if (classOverlap) throw new Exception("Lớp học đã có môn học khác bị trùng giờ trong ngày này.");
            }

            schedule.ClassSubjectId = dto.ClassSubjectId;
            schedule.DayOfWeek = dto.DayOfWeek;
            schedule.StartTime = dto.StartTime;
            schedule.EndTime = dto.EndTime;
            schedule.Room = dto.Room;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == id);
            if (schedule == null) return false;

            schedule.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
