using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.Data;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);
            if (user != null && user.PasswordHash == request.Password)
            {
                int? lecturerId = null;
                int? studentId = null;

                if (user.Role == AttendanceSystem.Data.Entities.Enums.Role.Lecturer)
                {
                    var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UserId == user.Id && !l.IsDeleted);
                    lecturerId = lecturer?.Id;
                }
                else if (user.Role == AttendanceSystem.Data.Entities.Enums.Role.Student)
                {
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id && !s.IsDeleted);
                    studentId = student?.Id;
                }

                return Ok(new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    Role = user.Role.ToString(),
                    LecturerId = lecturerId,
                    StudentId = studentId
                });
            }

            return Unauthorized("Invalid username or password");
        }
    }
}
