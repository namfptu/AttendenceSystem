using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Data;
using AttendanceSystem.Data.Entities;
using AttendanceSystem.Data.Entities.Enums;

namespace AttendanceSystem.Business.Services
{
    public class ClassStudentService : IClassStudentService
    {
        private readonly AppDbContext _context;

        public ClassStudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassStudentDto>> GetStudentsByClassAsync(int classId)
        {
            return await _context.ClassStudents
                .Include(cs => cs.Student).ThenInclude(s => s.User)
                .Include(cs => cs.Class)
                .Where(cs => cs.ClassId == classId && !cs.IsDeleted)
                .Select(cs => new ClassStudentDto
                {
                    Id = cs.Id,
                    ClassId = cs.ClassId,
                    StudentId = cs.StudentId,
                    EnrolledAt = cs.EnrolledAt,
                    Status = (int)cs.Status,
                    StudentCode = cs.Student.StudentCode,
                    StudentName = cs.Student.User.FullName,
                    ClassCode = cs.Class.ClassCode
                })
                .ToListAsync();
        }

        private async Task CheckStudentScheduleOverlapAsync(int classId, int studentId)
        {
            var targetClassSchedules = await _context.ClassSubjects
                .Where(csb => csb.ClassId == classId && csb.Status == ClassSubjectStatus.Active)
                .SelectMany(csb => csb.Schedules)
                .Where(s => !s.IsDeleted)
                .Select(s => new { s.ClassSubject.SemesterId, s.DayOfWeek, s.StartTime, s.EndTime })
                .ToListAsync();

            if (!targetClassSchedules.Any()) return;

            var studentClassIds = await _context.ClassStudents
                .Where(cs => cs.StudentId == studentId && cs.ClassId != classId && cs.Status == ClassStudentStatus.Active && !cs.IsDeleted)
                .Select(cs => cs.ClassId)
                .ToListAsync();

            if (!studentClassIds.Any()) return;

            var student = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == studentId);
            string studentName = student?.User?.FullName ?? "Sinh viên";

            foreach (var targetSched in targetClassSchedules)
            {
                var overlap = await _context.ClassSubjects
                    .Include(csb => csb.Class)
                    .Include(csb => csb.Subject)
                    .Where(csb => studentClassIds.Contains(csb.ClassId) 
                               && csb.SemesterId == targetSched.SemesterId 
                               && csb.Status == ClassSubjectStatus.Active)
                    .SelectMany(csb => csb.Schedules)
                    .Where(s => !s.IsDeleted && s.DayOfWeek == targetSched.DayOfWeek 
                             && s.StartTime < targetSched.EndTime && s.EndTime > targetSched.StartTime)
                    .Select(s => new { s.ClassSubject.Class.ClassCode, s.ClassSubject.Subject.SubjectName })
                    .FirstOrDefaultAsync();

                if (overlap != null)
                {
                    throw new InvalidOperationException($"Trùng lịch: Sinh viên {studentName} đã có lịch học trùng giờ ở lớp {overlap.ClassCode} ({overlap.SubjectName}).");
                }
            }
        }

        public async Task<ClassStudentDto> AddStudentToClassAsync(int classId, int studentId)
        {
            var exists = await _context.ClassStudents.AnyAsync(cs => cs.ClassId == classId && cs.StudentId == studentId && !cs.IsDeleted);
            if (exists)
            {
                throw new InvalidOperationException("Sinh viên này đã có trong lớp.");
            }

            await CheckStudentScheduleOverlapAsync(classId, studentId);

            var entity = new ClassStudent
            {
                ClassId = classId,
                StudentId = studentId,
                EnrolledAt = DateTime.UtcNow,
                Status = ClassStudentStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _context.ClassStudents.Add(entity);
            await _context.SaveChangesAsync();

            return new ClassStudentDto
            {
                Id = entity.Id,
                ClassId = entity.ClassId,
                StudentId = entity.StudentId,
                EnrolledAt = entity.EnrolledAt,
                Status = (int)entity.Status
            };
        }

        public async Task<bool> RemoveStudentFromClassAsync(int id)
        {
            var entity = await _context.ClassStudents.FirstOrDefaultAsync(cs => cs.Id == id);
            if (entity == null) return false;

            // Soft delete
            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int classId, int studentId)
        {
            return await _context.ClassStudents.AnyAsync(cs => cs.ClassId == classId && cs.StudentId == studentId && !cs.IsDeleted);
        }

        public async Task<ImportResultDto> ImportStudentsAsync(int classId, System.IO.Stream excelStream)
        {
            var result = new ImportResultDto();

            // Check if class exists
            var classObj = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId && !c.IsDeleted);
            if (classObj == null)
            {
                result.Errors.Add("Lớp học không tồn tại.");
                result.ErrorCount++;
                return result;
            }

            try
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook(excelStream);
                var worksheet = workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    result.Errors.Add("File Excel trống hoặc không đúng định dạng.");
                    result.ErrorCount++;
                    return result;
                }

                // Assuming column A is StudentCode
                var rows = worksheet.RowsUsed();
                var existingClassStudents = await _context.ClassStudents
                    .Where(cs => cs.ClassId == classId && !cs.IsDeleted)
                    .Select(cs => cs.StudentId)
                    .ToListAsync();

                bool isFirstRow = true;
                foreach (var row in rows)
                {
                    var studentCode = row.Cell(1).GetString()?.Trim();
                    if (string.IsNullOrEmpty(studentCode))
                    {
                        isFirstRow = false;
                        continue;
                    }

                    // Find student by code
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentCode == studentCode && !s.IsDeleted);
                    if (student == null)
                    {
                        if (isFirstRow)
                        {
                            // It's likely a header, ignore it silently
                            isFirstRow = false;
                            continue;
                        }

                        result.Errors.Add($"Dòng {row.RowNumber()}: Sinh viên mã '{studentCode}' không tồn tại trong hệ thống.");
                        result.ErrorCount++;
                        continue;
                    }

                    isFirstRow = false;

                    // Check if already in class
                    if (existingClassStudents.Contains(student.Id))
                    {
                        result.Errors.Add($"Dòng {row.RowNumber()}: Sinh viên mã '{studentCode}' đã có trong lớp này.");
                        result.ErrorCount++;
                        continue;
                    }

                    // Check schedule overlap
                    try
                    {
                        await CheckStudentScheduleOverlapAsync(classId, student.Id);
                    }
                    catch (InvalidOperationException ex)
                    {
                        result.Errors.Add($"Dòng {row.RowNumber()}: {ex.Message}");
                        result.ErrorCount++;
                        continue;
                    }

                    // Add to class
                    var entity = new ClassStudent
                    {
                        ClassId = classId,
                        StudentId = student.Id,
                        EnrolledAt = DateTime.UtcNow,
                        Status = ClassStudentStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.ClassStudents.Add(entity);
                    existingClassStudents.Add(student.Id); // prevent duplicate in same file
                    result.SuccessCount++;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Errors.Add("Lỗi khi đọc file Excel: " + ex.Message);
                result.ErrorCount++;
            }

            return result;
        }
    }
}
