using System;
using System.Collections.Generic;

namespace AttendanceSystem.Business.DTOs
{
    // ===================== STUDENT DASHBOARD =====================
    public class StudentDashboardDto
    {
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public List<StudentCourseAttendanceDto> Courses { get; set; } = new List<StudentCourseAttendanceDto>();
    }

    public class StudentCourseAttendanceDto
    {
        public int ClassSubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public string ClassName { get; set; }
        public string LecturerName { get; set; }
        public int CompletedSessions { get; set; }
        public int TotalSlots { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public double AbsentPercentage { get; set; }
        public bool IsBanned { get; set; } // true if absent > 20%
    }

    // ===================== LECTURER DASHBOARD =====================
    public class LecturerDashboardDto
    {
        public string LecturerName { get; set; }
        public List<LecturerTodayClassDto> TodayClasses { get; set; } = new List<LecturerTodayClassDto>();
        public List<LecturerClassSummaryDto> ClassSummaries { get; set; } = new List<LecturerClassSummaryDto>();
        public List<TopAbsentStudentDto> TopAbsentStudents { get; set; } = new List<TopAbsentStudentDto>();
    }

    public class LecturerTodayClassDto
    {
        public int ClassSubjectId { get; set; }
        public int ScheduleId { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; }
        public int? SessionId { get; set; }
        public string SessionStatus { get; set; }
    }

    public class LecturerClassSummaryDto
    {
        public int ClassSubjectId { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public int TotalStudents { get; set; }
        public int TotalSessions { get; set; }
        public double AvgAttendanceRate { get; set; }
    }

    public class TopAbsentStudentDto
    {
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public int AbsentCount { get; set; }
        public int TotalSessions { get; set; }
        public double AbsentPercentage { get; set; }
    }

    // ===================== ADMIN DASHBOARD =====================
    public class AdminDashboardDto
    {
        public int TotalStudents { get; set; }
        public int TotalLecturers { get; set; }
        public int TotalClasses { get; set; }
        public int TotalClassSubjects { get; set; }
        public int TodayOpenSessions { get; set; }
        public int TodayClosedSessions { get; set; }
        public double OverallAttendanceRate { get; set; }
        public List<LecturerClassSummaryDto> RecentClassSummaries { get; set; } = new List<LecturerClassSummaryDto>();
    }
}
