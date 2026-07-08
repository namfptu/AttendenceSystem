using System;

namespace AttendanceSystem.Business.DTOs
{
    public class ScheduleDto
    {
        public int Id { get; set; }
        public int ClassSubjectId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; }

        // Display fields
        public string? ClassName { get; set; }
        public string? SubjectName { get; set; }
        public string? LecturerName { get; set; }
        public string? SemesterName { get; set; }
        public string? ClassSubjectDisplay { get; set; }
    }
}
