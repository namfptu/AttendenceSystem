using System;

namespace AttendanceSystem.Business.DTOs
{
    public class ClassSubjectDto
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int LecturerId { get; set; }
        public int SemesterId { get; set; }
        public int Status { get; set; } // 0: Active, 1: Closed, 2: Cancelled

        // For display in UI
        public string? ClassCode { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public string? LecturerName { get; set; }
        public string? SemesterName { get; set; }
    }
}
