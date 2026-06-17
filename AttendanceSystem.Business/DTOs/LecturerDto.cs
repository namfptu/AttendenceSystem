namespace AttendanceSystem.Business.DTOs
{
    public class LecturerDto
    {
        public int Id { get; set; }
        public string LecturerCode { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
