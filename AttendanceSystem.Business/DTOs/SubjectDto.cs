namespace AttendanceSystem.Business.DTOs
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credits { get; set; }
        public int TotalSlots { get; set; }
        public string? Description { get; set; }
    }
}
