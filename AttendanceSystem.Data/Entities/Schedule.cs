using System;
using System.Collections.Generic;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Model đại diện cho một lịch học cố định của lớp học phần.
    /// (Ví dụ: Thứ 2, 7:00 - 9:00 tại phòng A1)
    /// </summary>
    public class Schedule : BaseEntity
    {
        public int Id { get; set; }

        public int ClassSubjectId { get; set; }

        /// <summary>
        /// Ngày trong tuần (0=Sunday, 1=Monday,...)
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string Room { get; set; }

        // Navigation properties
        public virtual ClassSubject ClassSubject { get; set; }
        
        /// <summary>
        /// Theo yêu cầu, AttendanceSession được link với Schedule để biết phiên điểm danh này cho buổi/lịch học nào.
        /// </summary>
        public virtual ICollection<AttendanceSession> AttendanceSessions { get; set; } = new List<AttendanceSession>();
    }
}
