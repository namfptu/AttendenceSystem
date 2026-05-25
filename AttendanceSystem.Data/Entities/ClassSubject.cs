using System.Collections.Generic;
using AttendanceSystem.Data.Entities.Enums;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Model đại diện cho Lớp Học Phần (Phân công lớp học môn gì, do ai dạy, trong kỳ nào).
    /// Đây là trung tâm kết nối Lớp hành chính, Môn học, Giảng viên và Học kỳ.
    /// </summary>
    public class ClassSubject : BaseEntity
    {
        public int Id { get; set; }

        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int LecturerId { get; set; }
        public int SemesterId { get; set; }

        /// <summary>
        /// Trạng thái của lớp học phần (Active, Closed, Cancelled).
        /// </summary>
        public ClassSubjectStatus Status { get; set; } = ClassSubjectStatus.Active;

        // Navigation properties
        public virtual Class Class { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual Lecturer Lecturer { get; set; }
        public virtual Semester Semester { get; set; }
        
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public virtual ICollection<AttendanceSession> AttendanceSessions { get; set; } = new List<AttendanceSession>();
    }
}
