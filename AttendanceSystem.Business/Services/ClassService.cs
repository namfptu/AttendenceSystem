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
    public class ClassService : IClassService
    {
        private readonly AppDbContext _context;

        public ClassService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassDto>> GetAllAsync()
        {
            return await _context.Classes
                .Where(c => !c.IsDeleted)
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    ClassCode = c.ClassCode,
                    ClassName = c.ClassName
                })
                .ToListAsync();
        }

        public async Task<ClassDto?> GetByIdAsync(int id)
        {
            var entity = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (entity == null) return null;

            return new ClassDto
            {
                Id = entity.Id,
                ClassCode = entity.ClassCode,
                ClassName = entity.ClassName
            };
        }

        public async Task<ClassDto> CreateAsync(ClassDto dto)
        {
            var entity = new Class
            {
                ClassCode = dto.ClassCode,
                ClassName = dto.ClassName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Classes.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<ClassDto?> UpdateAsync(int id, ClassDto dto)
        {
            var entity = await _context.Classes
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (entity == null) return null;

            entity.ClassCode = dto.ClassCode;
            entity.ClassName = dto.ClassName;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Classes.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByCodeAsync(string classCode, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Classes.AnyAsync(c => c.ClassCode == classCode && c.Id != excludeId.Value && !c.IsDeleted);
            }
            return await _context.Classes.AnyAsync(c => c.ClassCode == classCode && !c.IsDeleted);
        }
    }
}
