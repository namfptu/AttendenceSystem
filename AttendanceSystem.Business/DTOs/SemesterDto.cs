using System;

namespace AttendanceSystem.Business.DTOs
{
    public class SemesterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
