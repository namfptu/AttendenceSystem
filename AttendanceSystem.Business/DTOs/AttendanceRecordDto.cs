using System;

namespace AttendanceSystem.Business.DTOs
{
    public class AttendanceRecordDto
    {
        public int Id { get; set; }
        public int AttendanceSessionId { get; set; }
        public int StudentId { get; set; }
        public string? Status { get; set; }
        public DateTime? CheckInTime { get; set; }
        public bool IsManualEdited { get; set; }
        public int? EditedByLecturerId { get; set; }
        public DateTime? EditedAt { get; set; }
        public string? Note { get; set; }

        // Display fields
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
