using System;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Lớp cơ sở cho các Entity chính trong hệ thống.
    /// Cung cấp các trường Audit dùng chung theo yêu cầu thiết kế để tracking dữ liệu và hỗ trợ Soft Delete.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Thời điểm tạo bản ghi.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời điểm cập nhật bản ghi gần nhất.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Cờ đánh dấu bản ghi đã bị xóa mềm (Soft Delete).
        /// Hữu ích cho việc không xóa cứng dữ liệu thực tế khỏi DB.
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
