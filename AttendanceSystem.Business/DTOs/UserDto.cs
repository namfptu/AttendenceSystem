namespace AttendanceSystem.Business.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public int? LecturerId { get; set; }
        public int? StudentId { get; set; }
    }
}
