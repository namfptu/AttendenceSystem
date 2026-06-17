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
    public class SemesterService : ISemesterService
    {
        private readonly AppDbContext _context;

        public SemesterService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SemesterDto>> GetAllAsync()
        {
            return await _context.Semesters
                .Where(s => !s.IsDeleted)
                .Select(s => new SemesterDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate
                })
                .ToListAsync();
        }

        public async Task<SemesterDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Semesters
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (entity == null) return null;

            return new SemesterDto
            {
                Id = entity.Id,
                Name = entity.Name,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate
            };
        }

        public async Task<SemesterDto> CreateAsync(SemesterDto dto)
        {
            var entity = new Semester
            {
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Semesters.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<SemesterDto?> UpdateAsync(int id, SemesterDto dto)
        {
            var entity = await _context.Semesters
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (entity == null) return null;

            entity.Name = dto.Name;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Semesters.FirstOrDefaultAsync(s => s.Id == id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Semesters.AnyAsync(s => s.Name == name && s.Id != excludeId.Value && !s.IsDeleted);
            }
            return await _context.Semesters.AnyAsync(s => s.Name == name && !s.IsDeleted);
        }
    }
}
