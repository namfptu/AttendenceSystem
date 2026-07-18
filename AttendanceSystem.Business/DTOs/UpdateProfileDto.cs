using System;

namespace AttendanceSystem.Business.DTOs
{
    public class UpdateProfileDto
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
