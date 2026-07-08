using System.Collections.Generic;

namespace AttendanceSystem.Business.DTOs
{
    public class TakeAttendanceDto
    {
        public int AttendanceSessionId { get; set; }
        public List<AttendanceRecordDto> Records { get; set; } = new List<AttendanceRecordDto>();
    }
}
