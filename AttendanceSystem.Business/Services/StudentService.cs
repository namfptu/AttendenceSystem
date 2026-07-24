using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Data;
using AttendanceSystem.Data.Entities;

namespace AttendanceSystem.Business.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.User)
                .Where(s => !s.IsDeleted)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    StudentCode = s.StudentCode,
                    Faculty = s.Faculty,
                    Major = s.Major,
                    FullName = s.User.FullName,
                    Email = s.User.Email,
                    AvatarUrl = s.User.AvatarUrl
                })
                .ToListAsync();
        }

        public async Task<StudentDto?> GetByIdAsync(int id)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (student == null) return null;

            return new StudentDto
            {
                Id = student.Id,
                StudentCode = student.StudentCode,
                Faculty = student.Faculty,
                Major = student.Major,
                FullName = student.User.FullName,
                Email = student.User.Email,
                AvatarUrl = student.User.AvatarUrl
            };
        }

        public async Task<StudentDto> CreateAsync(StudentDto dto)
        {
            var user = new User
            {
                Username = dto.Email,
                PasswordHash = "123456", // In real app, hash this
                Email = dto.Email,
                FullName = dto.FullName,
                Role = AttendanceSystem.Data.Entities.Enums.Role.Student,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var student = new Student
            {
                StudentCode = dto.StudentCode,
                Faculty = dto.Faculty,
                Major = dto.Major,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            dto.Id = student.Id;
            return dto;
        }

        public async Task<StudentDto?> UpdateAsync(int id, StudentDto dto)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (student == null) return null;

            student.StudentCode = dto.StudentCode;
            student.Faculty = dto.Faculty;
            student.Major = dto.Major;
            student.UpdatedAt = DateTime.UtcNow;

            student.User.FullName = dto.FullName;
            student.User.Email = dto.Email;
            student.User.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            dto.AvatarUrl = student.User.AvatarUrl;
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return false;

            student.IsDeleted = true;
            student.User.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == excludeId.Value);
                if (student != null)
                {
                    return await _context.Users.AnyAsync(u => u.Email == email && u.Id != student.UserId && !u.IsDeleted);
                }
            }
            return await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<bool> ExistsByCodeAsync(string studentCode, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Students.AnyAsync(s => s.StudentCode == studentCode && s.Id != excludeId.Value && !s.IsDeleted);
            }
            return await _context.Students.AnyAsync(s => s.StudentCode == studentCode && !s.IsDeleted);
        }

        public async Task<ImportResultDto> ImportStudentsAsync(System.IO.Stream excelStream)
        {
            var result = new ImportResultDto();

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

                var existingEmails = await _context.Users.Where(u => !u.IsDeleted).Select(u => u.Email).ToListAsync();
                var existingCodes = await _context.Students.Where(s => !s.IsDeleted).Select(s => s.StudentCode).ToListAsync();

                var currentFileEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var currentFileCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                var rows = worksheet.RowsUsed();
                bool isFirstRow = true;

                foreach (var row in rows)
                {
                    var studentCode = row.Cell(1).GetString()?.Trim();
                    var fullName = row.Cell(2).GetString()?.Trim();
                    var email = row.Cell(3).GetString()?.Trim();
                    var faculty = row.Cell(4).GetString()?.Trim();
                    var major = row.Cell(5).GetString()?.Trim();

                    // If all columns are empty, skip
                    if (string.IsNullOrEmpty(studentCode) && string.IsNullOrEmpty(email))
                    {
                        isFirstRow = false;
                        continue;
                    }

                    // Check if likely a header row
                    if (isFirstRow && email != null && !email.Contains("@"))
                    {
                        isFirstRow = false;
                        continue;
                    }

                    isFirstRow = false;

                    // Validate required fields
                    if (string.IsNullOrEmpty(studentCode) || string.IsNullOrEmpty(fullName) || 
                        string.IsNullOrEmpty(email) || string.IsNullOrEmpty(faculty) || string.IsNullOrEmpty(major))
                    {
                        result.Errors.Add($"Dòng {row.RowNumber()}: Thiếu thông tin bắt buộc (Mã SV, Họ Tên, Email, Khoa, Ngành).");
                        result.ErrorCount++;
                        continue;
                    }

                    // Check duplicate in DB
                    if (existingEmails.Contains(email))
                    {
                        result.Errors.Add($"Dòng {row.RowNumber()}: Email '{email}' đã tồn tại trong hệ thống.");
                        result.ErrorCount++;
                        continue;
                    }
                    if (existingCodes.Contains(studentCode))
                    {
                        result.Errors.Add($"Dòng {row.RowNumber()}: Mã Sinh Viên '{studentCode}' đã tồn tại trong hệ thống.");
                        result.ErrorCount++;
                        continue;
                    }

                    // Check duplicate in current file
                    if (currentFileEmails.Contains(email))
                    {
                        result.Errors.Add($"Dòng {row.RowNumber()}: Email '{email}' bị trùng lặp trong chính file Excel.");
                        result.ErrorCount++;
                        continue;
                    }
                    if (currentFileCodes.Contains(studentCode))
                    {
                        result.Errors.Add($"Dòng {row.RowNumber()}: Mã Sinh Viên '{studentCode}' bị trùng lặp trong chính file Excel.");
                        result.ErrorCount++;
                        continue;
                    }

                    // Create User and Student
                    var user = new User
                    {
                        Username = email,
                        PasswordHash = "123456",
                        Email = email,
                        FullName = fullName,
                        Role = AttendanceSystem.Data.Entities.Enums.Role.Student,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Users.Add(user);

                    var student = new Student
                    {
                        StudentCode = studentCode,
                        Faculty = faculty,
                        Major = major,
                        User = user,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Students.Add(student);

                    currentFileEmails.Add(email);
                    currentFileCodes.Add(studentCode);
                    result.SuccessCount++;
                }

                if (result.SuccessCount > 0)
                {
                    await _context.SaveChangesAsync();
                }
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
