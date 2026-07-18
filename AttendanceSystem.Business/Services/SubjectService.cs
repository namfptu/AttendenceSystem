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
    public class SubjectService : ISubjectService
    {
        private readonly AppDbContext _context;

        public SubjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllAsync()
        {
            return await _context.Subjects
                .Where(s => !s.IsDeleted)
                .Select(s => new SubjectDto
                {
                    Id = s.Id,
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    Credits = s.Credits,
                    TotalSlots = s.TotalSlots,
                    Description = s.Description
                })
                .ToListAsync();
        }

        public async Task<SubjectDto?> GetByIdAsync(int id)
        {
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (subject == null) return null;

            return new SubjectDto
            {
                Id = subject.Id,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credits = subject.Credits,
                TotalSlots = subject.TotalSlots,
                Description = subject.Description
            };
        }

        public async Task<SubjectDto> CreateAsync(SubjectDto dto)
        {
            if (dto.TotalSlots < 1 || dto.TotalSlots > 20)
            {
                throw new InvalidOperationException("Tổng số slot phải nằm trong khoảng từ 1 đến 20.");
            }
            if (dto.Credits < 1 || dto.Credits > 10)
            {
                throw new InvalidOperationException("Số tín chỉ phải nằm trong khoảng từ 1 đến 10.");
            }

            var subject = new Subject
            {
                SubjectCode = dto.SubjectCode,
                SubjectName = dto.SubjectName,
                Credits = dto.Credits,
                TotalSlots = dto.TotalSlots > 0 ? dto.TotalSlots : 20,
                Description = dto.Description ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            dto.Id = subject.Id;
            return dto;
        }

        public async Task<SubjectDto?> UpdateAsync(int id, SubjectDto dto)
        {
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (subject == null) return null;

            if (dto.TotalSlots < 1 || dto.TotalSlots > 20)
            {
                throw new InvalidOperationException("Tổng số slot phải nằm trong khoảng từ 1 đến 20.");
            }
            if (dto.Credits < 1 || dto.Credits > 10)
            {
                throw new InvalidOperationException("Số tín chỉ phải nằm trong khoảng từ 1 đến 10.");
            }

            // Kiểm tra xem số slot mới có quá lớn so với các học kỳ hiện tại đang dạy không
            var activeAssignments = await _context.ClassSubjects
                .Include(cs => cs.Semester)
                .Where(cs => cs.SubjectId == id && !cs.IsDeleted)
                .ToListAsync();
            foreach (var cs in activeAssignments)
            {
                double weeks = (cs.Semester.EndDate - cs.Semester.StartDate).TotalDays / 7.0;
                if (weeks > 0 && dto.TotalSlots > weeks * 4)
                {
                    throw new InvalidOperationException($"Không thể đổi số slot thành {dto.TotalSlots} vì môn học này đang được phân công dạy ở học kỳ {cs.Semester.Name} ({Math.Round(weeks, 1)} tuần, tối đa {Math.Round(weeks * 4)} slots).");
                }
            }

            subject.SubjectCode = dto.SubjectCode;
            subject.SubjectName = dto.SubjectName;
            subject.Credits = dto.Credits;
            subject.TotalSlots = dto.TotalSlots > 0 ? dto.TotalSlots : 20;
            subject.Description = dto.Description ?? string.Empty;
            subject.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id);
            if (subject == null) return false;

            var isUsed = await _context.ClassSubjects.AnyAsync(cs => cs.SubjectId == id && !cs.IsDeleted);
            if (isUsed)
            {
                throw new InvalidOperationException("Không thể xóa môn học vì môn học này đã được phân công cho lớp học phần.");
            }

            subject.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByCodeAsync(string subjectCode, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Subjects.AnyAsync(s => s.SubjectCode == subjectCode && s.Id != excludeId.Value && !s.IsDeleted);
            }
            return await _context.Subjects.AnyAsync(s => s.SubjectCode == subjectCode && !s.IsDeleted);
        }
    }
}
