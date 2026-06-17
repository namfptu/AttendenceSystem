using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Data;
using AttendanceSystem.Data.Entities;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Business.Services
{
    public class LecturerService : ILecturerService
    {
        private readonly AppDbContext _context;

        public LecturerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LecturerDto>> GetAllAsync()
        {
            return await _context.Lecturers
                .Include(l => l.User)
                .Where(l => !l.IsDeleted)
                .Select(l => new LecturerDto
                {
                    Id = l.Id,
                    LecturerCode = l.LecturerCode,
                    Department = l.Department,
                    FullName = l.User.FullName,
                    Email = l.User.Email
                })
                .ToListAsync();
        }

        public async Task<LecturerDto?> GetByIdAsync(int id)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);

            if (lecturer == null) return null;

            return new LecturerDto
            {
                Id = lecturer.Id,
                LecturerCode = lecturer.LecturerCode,
                Department = lecturer.Department,
                FullName = lecturer.User.FullName,
                Email = lecturer.User.Email
            };
        }

        public async Task<LecturerDto> CreateAsync(LecturerDto dto)
        {
            var user = new User
            {
                Username = dto.Email,
                PasswordHash = "123456", // In real app, hash this
                Email = dto.Email,
                FullName = dto.FullName,
                Role = AttendanceSystem.Data.Entities.Enums.Role.Lecturer,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var lecturer = new Lecturer
            {
                LecturerCode = dto.LecturerCode,
                Department = dto.Department,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };
            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();

            dto.Id = lecturer.Id;
            return dto;
        }

        public async Task<LecturerDto?> UpdateAsync(int id, LecturerDto dto)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);

            if (lecturer == null) return null;

            lecturer.LecturerCode = dto.LecturerCode;
            lecturer.Department = dto.Department;
            lecturer.UpdatedAt = DateTime.UtcNow;

            lecturer.User.FullName = dto.FullName;
            lecturer.User.Email = dto.Email;
            lecturer.User.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted);

            if (lecturer == null) return false;

            lecturer.IsDeleted = true;
            lecturer.User.IsDeleted = true;
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null)
        {
            return await _context.Lecturers.AnyAsync(l => l.LecturerCode == code && !l.IsDeleted && l.Id != excludeId);
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
        {
            // For email we check all users except the user tied to this lecturer
            if (excludeId.HasValue)
            {
                var lecturer = await _context.Lecturers.FindAsync(excludeId.Value);
                if (lecturer != null)
                {
                    return await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted && u.Id != lecturer.UserId);
                }
            }
            return await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);
        }
    }
}
