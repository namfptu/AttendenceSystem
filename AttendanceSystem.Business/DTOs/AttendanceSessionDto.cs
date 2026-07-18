using System;

namespace AttendanceSystem.Business.DTOs
{
    public class AttendanceSessionDto
    {
        public int Id { get; set; }
        public int ClassSubjectId { get; set; }
        public int? ScheduleId { get; set; }
        public DateTime SessionDate { get; set; }
        public string? Title { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int LateAfterMinutes { get; set; } = 15;
        public string? Status { get; set; }
        public DateTime? OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int CreatedByLecturerId { get; set; }

        // Display fields
        public string? ClassName { get; set; }
        public string? SubjectName { get; set; }
        public string? LecturerName { get; set; }
        public string? Room { get; set; }
        public string? SemesterName { get; set; }
        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
    }
}
