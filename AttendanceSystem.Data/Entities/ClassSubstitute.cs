using System;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Bảng lưu trữ lịch phân công dạy thay (Substitute Lecturer).
    /// Hỗ trợ Admin gán quyền cho một giảng viên khác được phép mở phiên điểm danh cho lớp của đồng nghiệp.
    /// </summary>
    public class ClassSubstitute : BaseEntity
    {
        public int Id { get; set; }

        public int ClassSubjectId { get; set; }
        
        /// <summary>
        /// Giảng viên được phân công dạy thay.
        /// </summary>
        public int LecturerId { get; set; }

        /// <summary>
        /// Ngày được phân công dạy thay.
        /// </summary>
        public DateTime SubstituteDate { get; set; }

        /// <summary>
        /// Lý do dạy thay (Ví dụ: Thầy A ốm).
        /// </summary>
        public string Note { get; set; }

        // Navigation properties
        public virtual ClassSubject ClassSubject { get; set; }
        public virtual Lecturer Lecturer { get; set; }
    }
}
