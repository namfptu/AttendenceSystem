using System;
using System.Collections.Generic;

namespace AttendanceSystem.Data.Entities
{
    /// <summary>
    /// Model đại diện cho một học kỳ.
    /// </summary>
    public class Semester : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        // Navigation properties
        public virtual ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
}
