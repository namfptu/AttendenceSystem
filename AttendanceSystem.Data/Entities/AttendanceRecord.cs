using System;
using AttendanceSystem.Data.Entities.Enums;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Model đại diện cho bản ghi điểm danh của từng sinh viên trong một phiên.
    /// Được thiết kế hỗ trợ Audit (giảng viên sửa tay) và Anti-fraud (IP, Location).
    /// </summary>
    public class AttendanceRecord : BaseEntity
    {
        public int Id { get; set; }

        public int AttendanceSessionId { get; set; }

        public int StudentId { get; set; }

        /// <summary>
        /// Trạng thái điểm danh (Present, Late, Absent, Excused).
        /// </summary>
        public AttendanceStatus Status { get; set; }

        public DateTime? CheckInTime { get; set; }

        // Audit fields cho trường hợp giảng viên sửa tay
        public bool IsManualEdited { get; set; } = false;
        public int? EditedByLecturerId { get; set; }
        public DateTime? EditedAt { get; set; }
        public string Note { get; set; }

        // Navigation properties
        public virtual AttendanceSession AttendanceSession { get; set; }
        public virtual Student Student { get; set; }
        public virtual Lecturer EditedByLecturer { get; set; }
    }
}
