using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Data;
using AttendanceSystem.Business.DTOs;
using System.Linq;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetProfile(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null) return NotFound();

            int? lecturerId = null;
            int? studentId = null;

            if (user.Role == AttendanceSystem.Data.Entities.Enums.Role.Lecturer)
            {
                lecturerId = await _context.Lecturers.Where(l => l.UserId == user.Id && !l.IsDeleted).Select(l => l.Id).FirstOrDefaultAsync();
                if (lecturerId == 0) lecturerId = null;
            }
            else if (user.Role == AttendanceSystem.Data.Entities.Enums.Role.Student)
            {
                studentId = await _context.Students.Where(s => s.UserId == user.Id && !s.IsDeleted).Select(s => s.Id).FirstOrDefaultAsync();
                if (studentId == 0) studentId = null;
            }

            return Ok(new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                Email = user.Email,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                LecturerId = lecturerId,
                StudentId = studentId
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
            if (dto.Phone != null) user.Phone = dto.Phone;
            if (dto.AvatarUrl != null) user.AvatarUrl = dto.AvatarUrl;

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}
