using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using AttendanceSystem.Data;
using AttendanceSystem.Data.Entities.Enums;

namespace AttendanceSystem.Business.Services
{
    public class ExportService : IExportService
    {
        private readonly AppDbContext _context;
        public ExportService(AppDbContext context) { _context = context; }

        public async Task<byte[]> ExportAttendanceByClassSubjectAsync(int classSubjectId)
        {
            var cs = await _context.ClassSubjects
                .Include(c => c.Class).Include(c => c.Subject).Include(c => c.Lecturer).ThenInclude(l => l.User).Include(c => c.Semester)
                .FirstOrDefaultAsync(c => c.Id == classSubjectId && !c.IsDeleted);
            if (cs == null) return null;

            var sessions = await _context.AttendanceSessions
                .Where(s => s.ClassSubjectId == classSubjectId && s.Status == SessionStatus.Closed && !s.IsDeleted)
                .OrderBy(s => s.SessionDate).ToListAsync();

            var students = await _context.ClassStudents
                .Include(s => s.Student).ThenInclude(s => s.User)
                .Where(s => s.ClassId == cs.ClassId && !s.IsDeleted && s.Status == ClassStudentStatus.Active)
                .OrderBy(s => s.Student.StudentCode).ToListAsync();

            var allRecords = await _context.AttendanceRecords
                .Where(r => r.AttendanceSession.ClassSubjectId == classSubjectId && !r.IsDeleted)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Attendance");

            // Header info
            ws.Cell(1, 1).Value = $"Class: {cs.Class.ClassCode} - {cs.Class.ClassName}";
            ws.Cell(2, 1).Value = $"Subject: {cs.Subject.SubjectCode} - {cs.Subject.SubjectName}";
            ws.Cell(3, 1).Value = $"Lecturer: {cs.Lecturer.User.FullName}";
            ws.Cell(4, 1).Value = $"Semester: {cs.Semester.Name}";
            ws.Range(1, 1, 4, 1).Style.Font.Bold = true;

            // Table headers
            int headerRow = 6;
            ws.Cell(headerRow, 1).Value = "No.";
            ws.Cell(headerRow, 2).Value = "Student Code";
            ws.Cell(headerRow, 3).Value = "Full Name";
            for (int i = 0; i < sessions.Count; i++)
            {
                ws.Cell(headerRow, 4 + i).Value = sessions[i].SessionDate.ToString("dd/MM");
            }
            ws.Cell(headerRow, 4 + sessions.Count).Value = "Absent";
            ws.Cell(headerRow, 5 + sessions.Count).Value = "%";
            var headerRange = ws.Range(headerRow, 1, headerRow, 5 + sessions.Count);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // Data rows
            for (int si = 0; si < students.Count; si++)
            {
                int row = headerRow + 1 + si;
                var st = students[si];
                ws.Cell(row, 1).Value = si + 1;
                ws.Cell(row, 2).Value = st.Student.StudentCode;
                ws.Cell(row, 3).Value = st.Student.User.FullName;

                int absentCount = 0;
                for (int j = 0; j < sessions.Count; j++)
                {
                    var record = allRecords.FirstOrDefault(r => r.AttendanceSessionId == sessions[j].Id && r.StudentId == st.StudentId);
                    string statusStr = record != null ? StatusToShort(record.Status) : "-";
                    ws.Cell(row, 4 + j).Value = statusStr;
                    if (record?.Status == AttendanceStatus.Absent) absentCount++;
                }
                ws.Cell(row, 4 + sessions.Count).Value = absentCount;
                double pct = sessions.Count > 0 ? Math.Round((double)absentCount / sessions.Count * 100, 1) : 0;
                ws.Cell(row, 5 + sessions.Count).Value = pct;
                if (pct > 20) ws.Row(row).Style.Font.FontColor = XLColor.Red;
            }

            ws.Columns().AdjustToContents();

            using var stream = new System.IO.MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static string StatusToShort(AttendanceStatus status)
        {
            return status switch
            {
                AttendanceStatus.Present => "P",
                AttendanceStatus.Late => "L",
                AttendanceStatus.Absent => "A",
                AttendanceStatus.Excused => "E",
                _ => "-"
            };
        }
    }
}
