using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Data;
using AttendanceSystem.Data.Entities.Enums;

namespace AttendanceSystem.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;
        public DashboardService(AppDbContext context) { _context = context; }

        public async Task<StudentDashboardDto> GetStudentDashboardAsync(int studentId)
        {
            var student = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);
            if (student == null) return null;

            var classIds = await _context.ClassStudents
                .Where(cs => cs.StudentId == studentId && !cs.IsDeleted && cs.Status == ClassStudentStatus.Active)
                .Select(cs => cs.ClassId).ToListAsync();

            var classSubjects = await _context.ClassSubjects
                .Include(cs => cs.Class).Include(cs => cs.Subject).Include(cs => cs.Lecturer).ThenInclude(l => l.User).Include(cs => cs.Semester)
                .Where(cs => classIds.Contains(cs.ClassId) && !cs.IsDeleted && cs.Status == ClassSubjectStatus.Active)
                .ToListAsync();

            var courses = new List<StudentCourseAttendanceDto>();
            foreach (var cs in classSubjects)
            {
                var sessions = await _context.AttendanceSessions
                    .Where(s => s.ClassSubjectId == cs.Id && s.Status == SessionStatus.Closed && !s.IsDeleted)
                    .Select(s => s.Id).ToListAsync();

                var records = await _context.AttendanceRecords
                    .Where(r => sessions.Contains(r.AttendanceSessionId) && r.StudentId == studentId && !r.IsDeleted)
                    .ToListAsync();

                int completed = sessions.Count;
                int totalSlots = cs.Subject.TotalSlots > 0 ? cs.Subject.TotalSlots : 20; // fallback to 20
                int absent = records.Count(r => r.Status == AttendanceStatus.Absent);
                double pct = Math.Round((double)absent / totalSlots * 100, 1);

                courses.Add(new StudentCourseAttendanceDto
                {
                    ClassSubjectId = cs.Id, SubjectName = cs.Subject.SubjectName, SubjectCode = cs.Subject.SubjectCode,
                    ClassName = cs.Class.ClassName, LecturerName = cs.Lecturer.User.FullName,
                    CompletedSessions = completed,
                    TotalSlots = totalSlots,
                    PresentCount = records.Count(r => r.Status == AttendanceStatus.Present),
                    AbsentCount = absent,
                    AbsentPercentage = pct, IsBanned = pct > 20
                });
            }

            return new StudentDashboardDto { StudentName = student.User.FullName, StudentCode = student.StudentCode, Courses = courses };
        }

        public async Task<LecturerDashboardDto> GetLecturerDashboardAsync(int lecturerId)
        {
            var lecturer = await _context.Lecturers.Include(l => l.User).FirstOrDefaultAsync(l => l.Id == lecturerId && !l.IsDeleted);
            if (lecturer == null) return null;

            var today = DateTime.UtcNow.Date;
            var todayDow = DateTime.UtcNow.DayOfWeek;

            // Today classes from schedules
            var schedules = await _context.Schedules
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Class)
                .Include(s => s.ClassSubject).ThenInclude(cs => cs.Subject)
                .Where(s => s.ClassSubject.LecturerId == lecturerId && s.DayOfWeek == todayDow && !s.IsDeleted && s.ClassSubject.Status == ClassSubjectStatus.Active)
                .ToListAsync();

            var todayClasses = new List<LecturerTodayClassDto>();
            foreach (var sch in schedules)
            {
                var session = await _context.AttendanceSessions
                    .FirstOrDefaultAsync(s => s.ClassSubjectId == sch.ClassSubjectId 
                                           && s.SessionDate.Date == today 
                                           && (s.ScheduleId == sch.Id || (s.ScheduleId == null && s.StartTime == sch.StartTime))
                                           && !s.IsDeleted);

                todayClasses.Add(new LecturerTodayClassDto
                {
                    ClassSubjectId = sch.ClassSubjectId, 
                    ScheduleId = sch.Id,
                    ClassName = sch.ClassSubject.Class.ClassName,
                    SubjectName = sch.ClassSubject.Subject.SubjectName, StartTime = sch.StartTime,
                    EndTime = sch.EndTime, Room = sch.Room,
                    SessionId = session?.Id, SessionStatus = session?.Status.ToString()
                });
            }

            // Class summaries
            var myClassSubjects = await _context.ClassSubjects
                .Include(cs => cs.Class).Include(cs => cs.Subject)
                .Where(cs => cs.LecturerId == lecturerId && !cs.IsDeleted && cs.Status == ClassSubjectStatus.Active)
                .ToListAsync();

            var summaries = new List<LecturerClassSummaryDto>();
            foreach (var cs in myClassSubjects)
            {
                int totalStudents = await _context.ClassStudents.CountAsync(s => s.ClassId == cs.ClassId && !s.IsDeleted && s.Status == ClassStudentStatus.Active);
                int totalSessions = await _context.AttendanceSessions.CountAsync(s => s.ClassSubjectId == cs.Id && s.Status == SessionStatus.Closed && !s.IsDeleted);
                int totalPresent = 0;
                if (totalSessions > 0 && totalStudents > 0)
                {
                    totalPresent = await _context.AttendanceRecords
                        .CountAsync(r => r.AttendanceSession.ClassSubjectId == cs.Id && r.Status == AttendanceStatus.Present && !r.IsDeleted);
                }
                double rate = (totalSessions > 0 && totalStudents > 0) ? Math.Round((double)totalPresent / (totalSessions * totalStudents) * 100, 1) : 0;

                summaries.Add(new LecturerClassSummaryDto
                {
                    ClassSubjectId = cs.Id, ClassName = cs.Class.ClassName, SubjectName = cs.Subject.SubjectName,
                    TotalStudents = totalStudents, TotalSessions = totalSessions, AvgAttendanceRate = rate
                });
            }

            return new LecturerDashboardDto { LecturerName = lecturer.User.FullName, TodayClasses = todayClasses, ClassSummaries = summaries };
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            var today = DateTime.UtcNow.Date;
            return new AdminDashboardDto
            {
                TotalStudents = await _context.Students.CountAsync(s => !s.IsDeleted),
                TotalLecturers = await _context.Lecturers.CountAsync(l => !l.IsDeleted),
                TotalClasses = await _context.Classes.CountAsync(c => !c.IsDeleted),
                TotalClassSubjects = await _context.ClassSubjects.CountAsync(cs => !cs.IsDeleted),
                TodayOpenSessions = await _context.AttendanceSessions.CountAsync(s => s.SessionDate.Date == today && s.Status == SessionStatus.Open && !s.IsDeleted),
                TodayClosedSessions = await _context.AttendanceSessions.CountAsync(s => s.SessionDate.Date == today && s.Status == SessionStatus.Closed && !s.IsDeleted)
            };
        }
    }
}
