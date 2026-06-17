using System;
using System.Collections.Generic;
using AttendanceSystem.Data.Entities.Enums;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Model đại diện cho một phiên điểm danh.
    /// Đã được tái cấu trúc mạnh theo yêu cầu Workflow Điểm danh:
    /// - Liên kết với ScheduleId
    /// - Thêm Lifecycle fields (OpenedAt, ClosedAt, QrExpiredAt)
    /// - Thêm tính năng chống gian lận (AllowedLatitude, AllowedLongitude, AllowedRadiusMeters)
    /// </summary>
    public class AttendanceSession : BaseEntity
    {
        public int Id { get; set; }

        public int ClassSubjectId { get; set; }
        
        /// <summary>
        /// Khóa ngoại trỏ về lịch học cụ thể. Có thể NULL nếu là buổi học bù (Make-up class).
        /// </summary>
        public int? ScheduleId { get; set; }

        public DateTime SessionDate { get; set; }
        public string Title { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// Số phút cho phép đi muộn (ví dụ 15 phút đầu giờ).
        /// </summary>
        public int LateAfterMinutes { get; set; }

        /// <summary>
        /// Trạng thái của phiên (Pending, Open, Closed).
        /// </summary>
        public SessionStatus Status { get; set; } = SessionStatus.Pending;

        // Lifecycle fields
        public DateTime? OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public int CreatedByLecturerId { get; set; }
        
        // (Đã loại bỏ các trường QR Code và GPS để chuyển về điểm danh thủ công toàn phần)

        // Navigation properties
        public virtual ClassSubject ClassSubject { get; set; }
        public virtual Schedule Schedule { get; set; }
        public virtual Lecturer CreatedByLecturer { get; set; }
        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}
