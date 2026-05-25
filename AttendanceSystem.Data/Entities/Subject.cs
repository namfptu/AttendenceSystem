using System.Collections.Generic;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Model đại diện cho một môn học.
    /// Được bổ sung cờ IsDeleted thông qua BaseEntity.
    /// </summary>
    public class Subject : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Mã môn học (Unique).
        /// </summary>
        public string SubjectCode { get; set; }

        /// <summary>
        /// Tên môn học.
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// Số tín chỉ.
        /// </summary>
        public int Credits { get; set; }

        public string Description { get; set; }

        // Navigation properties
        public virtual ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
}
