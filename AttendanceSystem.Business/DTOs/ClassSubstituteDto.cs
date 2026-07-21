using System;

namespace AttendanceSystem.Business.DTOs
{
    public class ClassSubstituteDto
    {
        public int Id { get; set; }
        public int ClassSubjectId { get; set; }
        public int LecturerId { get; set; }
        public DateTime SubstituteDate { get; set; }
        public string Note { get; set; }

        // Extra Display Properties
        public string ClassCode { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string PrimaryLecturerName { get; set; }
        public string SubstituteLecturerName { get; set; }
    }
}
