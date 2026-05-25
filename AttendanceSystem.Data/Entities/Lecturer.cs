using System.Collections.Generic;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Model đại diện cho giảng viên.
    /// Đã loại bỏ FullName và Email (chuyển sang User) để tránh lặp dữ liệu.
    /// Được bổ sung cờ IsDeleted thông qua BaseEntity.
    /// </summary>
    public class Lecturer : BaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// Mã số giảng viên (Unique).
        /// </summary>
        public string LecturerCode { get; set; }

        /// <summary>
        /// Phòng ban / Bộ môn của giảng viên.
        /// </summary>
        public string Department { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
}
