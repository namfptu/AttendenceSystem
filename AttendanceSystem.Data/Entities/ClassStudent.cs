using System;
using AttendanceSystem.Data.Entities.Enums;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Model đại diện cho sinh viên đăng ký lớp học phần (N-N).
    /// Cần cấu hình Unique cho cặp (CourseClassId, StudentId).
    /// Đã bổ sung Status để biết sinh viên đang học hay đã rớt/hủy môn.
    /// </summary>
    public class ClassStudent : BaseEntity
    {
        public int Id { get; set; }

        public int ClassId { get; set; }

        public int StudentId { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Trạng thái học của sinh viên trong lớp (Active, Dropped, Completed).
        /// </summary>
        public ClassStudentStatus Status { get; set; } = ClassStudentStatus.Active;

        // Navigation properties
        public virtual Class Class { get; set; }
        public virtual Student Student { get; set; }
    }
}
