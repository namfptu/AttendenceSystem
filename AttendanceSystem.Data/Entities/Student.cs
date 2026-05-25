using System.Collections.Generic;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Model đại diện cho sinh viên.
    /// Đã loại bỏ FullName và Email (chuyển sang User) để tránh lặp dữ liệu.
    /// Được bổ sung cờ IsDeleted thông qua BaseEntity.
    /// </summary>
    public class Student : BaseEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// Mã số sinh viên (Unique).
        /// </summary>
        public string StudentCode { get; set; }

        /// <summary>
        /// Khoa mà sinh viên đang theo học.
        /// </summary>
        public string Faculty { get; set; }

        /// <summary>
        /// Chuyên ngành của sinh viên.
        /// </summary>
        public string Major { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();
        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}
