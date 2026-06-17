using System.Collections.Generic;

namespace AttendanceSystem.Business.DTOs
{
    public class ImportResultDto
    {
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
