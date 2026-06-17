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
                Description = subject.Description
            };
        }

        public async Task<SubjectDto> CreateAsync(SubjectDto dto)
        {
            var subject = new Subject
            {
                SubjectCode = dto.SubjectCode,
                SubjectName = dto.SubjectName,
                Credits = dto.Credits,
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

            subject.SubjectCode = dto.SubjectCode;
            subject.SubjectName = dto.SubjectName;
            subject.Credits = dto.Credits;
            subject.Description = dto.Description ?? string.Empty;
            subject.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id);
            if (subject == null) return false;

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
