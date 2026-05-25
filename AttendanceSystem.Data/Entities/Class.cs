using System.Collections.Generic;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Model đại diện cho một Lớp hành chính (VD: SE1801).
    /// Lớp này chứa danh sách sinh viên cố định và sẽ đăng ký học nhiều môn (ClassSubject).
    /// </summary>
    public class Class : BaseEntity
    {
        public int Id { get; set; }

        /// <summary>
        /// Mã lớp hành chính (Unique). VD: SE1801.
        /// </summary>
        public string ClassCode { get; set; }

        /// <summary>
        /// Tên lớp hành chính.
        /// </summary>
        public string ClassName { get; set; }

        // Navigation properties
        public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();
        public virtual ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
}
