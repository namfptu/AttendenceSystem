using System.Collections.Generic;
using AttendanceSystem.Data.Entities.Enums;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Model đại diện cho một tài khoản trong hệ thống.
    /// Theo yêu cầu refactor, các thông tin chung như FullName, Email được gom về đây thay vì lặp lại ở bảng Student hay Lecturer.
    /// </summary>
    public class User : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Tên đăng nhập (Unique).
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Email người dùng (Unique). Đã được chuyển từ Student/Lecturer lên đây.
        /// </summary>
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        /// <summary>
        /// Họ và tên đầy đủ. Đã được chuyển từ Student/Lecturer lên đây.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Link ảnh thẻ để giảng viên đối chiếu khi điểm danh.
        /// </summary>
        public string? AvatarUrl { get; set; }

        public string? Phone { get; set; }

        /// <summary>
        /// Vai trò của user: Admin, Lecturer, Student.
        /// </summary>
        public Role Role { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Lecturer Lecturer { get; set; }
    }
}
