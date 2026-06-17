namespace AttendanceSystem.Business.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string StudentCode { get; set; } = null!;
        public string Faculty { get; set; } = null!;
        public string Major { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
