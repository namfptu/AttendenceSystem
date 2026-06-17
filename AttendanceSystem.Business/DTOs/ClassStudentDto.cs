using System;

namespace AttendanceSystem.Business.DTOs
{
    public class ClassStudentDto
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int StudentId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public int Status { get; set; } // 0: Active, 1: Dropped, 2: Completed

        // Additional information for display
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public string? ClassCode { get; set; }
    }
}
