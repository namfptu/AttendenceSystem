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
    public class AttendanceSessionService : IAttendanceSessionService
    {
        private readonly AppDbContext _context;

        public AttendanceSessionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendanceSessionDto>> GetAllAsync()
        {
            TimeZoneInfo vietnamZone;
            try
            {
                vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            }
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamZone);
            var today = localTime.Date;
            var todayDow = localTime.DayOfWeek;

            var dbSessions = await QuerySessions()
                .OrderByDescending(s => s.SessionDate)
                .ThenByDescending(s => s.StartTime)
                .Select(s => MapToDto(s))
                .ToListAsync();

            var todaySchedules = await _context.Schedules
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Class)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Subject)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Lecturer).ThenInclude(l => l.User)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Semester)
                .Where(s => s.DayOfWeek == todayDow && !s.IsDeleted && s.ClassSubject.Status == ClassSubjectStatus.Active)
                .ToListAsync();

            var result = new List<AttendanceSessionDto>(dbSessions);

            foreach (var sch in todaySchedules)
            {
                bool exists = dbSessions.Any(s => s.ClassSubjectId == sch.ClassSubjectId 
                                               && s.SessionDate.Date == today 
                                               && (s.ScheduleId == sch.Id || (s.ScheduleId == null && s.StartTime == sch.StartTime)));
                if (!exists)
                {
                    result.Add(new AttendanceSessionDto
                    {
                        Id = 0,
                        ClassSubjectId = sch.ClassSubjectId,
                        ScheduleId = sch.Id,
                        SessionDate = today,
                        Title = $"Lớp học {today:dd/MM/yyyy}",
                        StartTime = sch.StartTime,
                        EndTime = sch.EndTime,
                        LateAfterMinutes = 15,
                        Status = SessionStatus.Pending.ToString(),
                        ClassName = sch.ClassSubject.Class.ClassName,
                        SubjectName = sch.ClassSubject.Subject.SubjectName,
                        LecturerName = sch.ClassSubject.Lecturer.User.FullName,
                        Room = sch.Room,
                        SemesterName = sch.ClassSubject.Semester.Name,
                        TotalStudents = 0,
                        PresentCount = 0,
                        AbsentCount = 0
                    });
                }
            }

            return result.OrderByDescending(s => s.SessionDate).ThenByDescending(s => s.StartTime);
        }

        public async Task<IEnumerable<AttendanceSessionDto>> GetByLecturerIdAsync(int lecturerId)
        {
            TimeZoneInfo vietnamZone;
            try
            {
                vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            }
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamZone);
            var today = localTime.Date;
            var todayDow = localTime.DayOfWeek;

            var dbSessions = await QuerySessions()
                .Where(s => s.ClassSubject.LecturerId == lecturerId)
                .OrderByDescending(s => s.SessionDate)
                .ThenByDescending(s => s.StartTime)
                .Select(s => MapToDto(s))
                .ToListAsync();

            var todaySchedules = await _context.Schedules
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Class)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Subject)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Lecturer).ThenInclude(l => l.User)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Semester)
                .Where(s => s.ClassSubject.LecturerId == lecturerId && s.DayOfWeek == todayDow && !s.IsDeleted && s.ClassSubject.Status == ClassSubjectStatus.Active)
                .ToListAsync();

            var result = new List<AttendanceSessionDto>(dbSessions);

            foreach (var sch in todaySchedules)
            {
                bool exists = dbSessions.Any(s => s.ClassSubjectId == sch.ClassSubjectId 
                                               && s.SessionDate.Date == today 
                                               && (s.ScheduleId == sch.Id || (s.ScheduleId == null && s.StartTime == sch.StartTime)));
                if (!exists)
                {
                    result.Add(new AttendanceSessionDto
                    {
                        Id = 0,
                        ClassSubjectId = sch.ClassSubjectId,
                        ScheduleId = sch.Id,
                        SessionDate = today,
                        Title = $"Lớp học {today:dd/MM/yyyy}",
                        StartTime = sch.StartTime,
                        EndTime = sch.EndTime,
                        LateAfterMinutes = 15,
                        Status = SessionStatus.Pending.ToString(),
                        ClassName = sch.ClassSubject.Class.ClassName,
                        SubjectName = sch.ClassSubject.Subject.SubjectName,
                        LecturerName = sch.ClassSubject.Lecturer.User.FullName,
                        Room = sch.Room,
                        SemesterName = sch.ClassSubject.Semester.Name,
                        TotalStudents = 0,
                        PresentCount = 0,
                        AbsentCount = 0
                    });
                }
            }

            return result.OrderByDescending(s => s.SessionDate).ThenByDescending(s => s.StartTime);
        }

        public async Task<IEnumerable<AttendanceSessionDto>> GetTodaySessionsByLecturerAsync(int lecturerId)
        {
            TimeZoneInfo vietnamZone;
            try
            {
                vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            }
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamZone);
            var today = localTime.Date;
            return await QuerySessions()
                .Where(s => s.ClassSubject.LecturerId == lecturerId && s.SessionDate.Date == today)
                .OrderBy(s => s.StartTime)
                .Select(s => MapToDto(s))
                .ToListAsync();
        }

        public async Task<AttendanceSessionDto> GetByIdAsync(int id)
        {
            var session = await QuerySessions()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return null;
            return MapToDto(session);
        }

        public async Task<AttendanceSessionDto> CreateAsync(AttendanceSessionDto dto)
        {
            if (dto.StartTime >= dto.EndTime)
                throw new InvalidOperationException("StartTime must be before EndTime.");

            var classSubject = await _context.ClassSubjects
                .Include(cs => cs.Semester)
                .Include(cs => cs.Subject)
                .FirstOrDefaultAsync(cs => cs.Id == dto.ClassSubjectId && !cs.IsDeleted);
                
            if (classSubject == null)
                throw new InvalidOperationException("ClassSubject not found or deleted.");

            if (dto.SessionDate.Date < classSubject.Semester.StartDate.Date || dto.SessionDate.Date > classSubject.Semester.EndDate.Date)
                throw new InvalidOperationException($"Session date must be within semester dates ({classSubject.Semester.StartDate:dd/MM/yyyy} - {classSubject.Semester.EndDate:dd/MM/yyyy}).");

            var existingCount = await _context.AttendanceSessions
                .CountAsync(s => s.ClassSubjectId == dto.ClassSubjectId && !s.IsDeleted);
            var totalSlots = classSubject.Subject.TotalSlots > 0 ? classSubject.Subject.TotalSlots : 20;
            if (existingCount >= totalSlots)
                throw new InvalidOperationException($"Cannot create more than {totalSlots} sessions for this subject.");

            var overlap = await _context.AttendanceSessions
                .AnyAsync(s => s.ClassSubjectId == dto.ClassSubjectId 
                            && s.SessionDate.Date == dto.SessionDate.Date 
                            && !s.IsDeleted
                            && ((dto.StartTime >= s.StartTime && dto.StartTime < s.EndTime) 
                             || (dto.EndTime > s.StartTime && dto.EndTime <= s.EndTime)
                             || (dto.StartTime <= s.StartTime && dto.EndTime >= s.EndTime)));
            if (overlap)
                throw new InvalidOperationException("Time overlaps with an existing session of this class on the same day.");

            if (dto.ScheduleId.HasValue)
            {
                var existing = await _context.AttendanceSessions
                    .AnyAsync(s => s.ClassSubjectId == dto.ClassSubjectId 
                                && s.ScheduleId == dto.ScheduleId 
                                && s.SessionDate.Date == dto.SessionDate.Date 
                                && !s.IsDeleted);
                if (existing)
                {
                    throw new InvalidOperationException("A session for this schedule and date already exists.");
                }
            }

            var session = new AttendanceSession
            {
                ClassSubjectId = dto.ClassSubjectId,
                ScheduleId = dto.ScheduleId,
                SessionDate = dto.SessionDate,
                Title = dto.Title,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                LateAfterMinutes = dto.LateAfterMinutes,
                Status = SessionStatus.Pending,
                CreatedByLecturerId = dto.CreatedByLecturerId
            };

            _context.AttendanceSessions.Add(session);
            await _context.SaveChangesAsync();

            dto.Id = session.Id;
            dto.Status = SessionStatus.Pending.ToString();
            return dto;
        }

        public async Task<bool> OpenSessionAsync(int id)
        {
            var session = await _context.AttendanceSessions.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            if (session == null) return false;
            if (session.Status != SessionStatus.Pending) return false;

            session.Status = SessionStatus.Open;
            session.OpenedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CloseSessionAsync(int id)
        {
            var session = await _context.AttendanceSessions
                .Include(s => s.ClassSubject)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (session == null) return false;
            if (session.Status != SessionStatus.Open) return false;

            // Lấy tất cả sinh viên thuộc lớp hành chính của ClassSubject này
            var classStudentIds = await _context.ClassStudents
                .Where(cs => cs.ClassId == session.ClassSubject.ClassId && !cs.IsDeleted && cs.Status == ClassStudentStatus.Active)
                .Select(cs => cs.StudentId)
                .ToListAsync();

            // Lấy danh sách sinh viên đã có AttendanceRecord trong phiên này
            var recordedStudentIds = await _context.AttendanceRecords
                .Where(ar => ar.AttendanceSessionId == id && !ar.IsDeleted)
                .Select(ar => ar.StudentId)
                .ToListAsync();

            // Sinh viên chưa có record → Auto Absent
            var missingStudentIds = classStudentIds.Except(recordedStudentIds).ToList();
            foreach (var studentId in missingStudentIds)
            {
                _context.AttendanceRecords.Add(new AttendanceRecord
                {
                    AttendanceSessionId = id,
                    StudentId = studentId,
                    Status = AttendanceStatus.Absent,
                    CheckInTime = null,
                    IsManualEdited = false,
                    Note = "Auto-marked absent when session closed"
                });
            }

            session.Status = SessionStatus.Closed;
            session.ClosedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // === PRIVATE HELPERS ===

        private IQueryable<AttendanceSession> QuerySessions()
        {
            return _context.AttendanceSessions
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Class)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Subject)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Lecturer).ThenInclude(l => l.User)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Semester)
                .Include(s => s.Schedule)
                .Include(s => s.AttendanceRecords)
                .Where(s => !s.IsDeleted);
        }

        private static AttendanceSessionDto MapToDto(AttendanceSession s)
        {
            return new AttendanceSessionDto
            {
                Id = s.Id,
                ClassSubjectId = s.ClassSubjectId,
                ScheduleId = s.ScheduleId,
                SessionDate = s.SessionDate,
                Title = s.Title,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                LateAfterMinutes = s.LateAfterMinutes,
                Status = s.Status.ToString(),
                OpenedAt = s.OpenedAt,
                ClosedAt = s.ClosedAt,
                CreatedByLecturerId = s.CreatedByLecturerId,
                ClassName = s.ClassSubject?.Class?.ClassName,
                SubjectName = s.ClassSubject?.Subject?.SubjectName,
                LecturerName = s.ClassSubject?.Lecturer?.User?.FullName,
                Room = s.Schedule?.Room,
                SemesterName = s.ClassSubject?.Semester?.Name,
                TotalStudents = 0, // Computed from ClassStudents externally if needed
                PresentCount = s.AttendanceRecords?.Count(r => r.Status == AttendanceStatus.Present && !r.IsDeleted) ?? 0,
                AbsentCount = s.AttendanceRecords?.Count(r => r.Status == AttendanceStatus.Absent && !r.IsDeleted) ?? 0
            };
        }
    }
}
