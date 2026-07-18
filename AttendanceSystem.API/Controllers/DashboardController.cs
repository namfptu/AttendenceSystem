using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        public DashboardController(IDashboardService service) { _service = service; }

        [HttpGet("Student/{studentId}")]
        public async Task<ActionResult<StudentDashboardDto>> GetStudentDashboard(int studentId)
        {
            var dto = await _service.GetStudentDashboardAsync(studentId);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("Lecturer/{lecturerId}")]
        public async Task<ActionResult<LecturerDashboardDto>> GetLecturerDashboard(int lecturerId)
        {
            var dto = await _service.GetLecturerDashboardAsync(lecturerId);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("Admin")]
        public async Task<ActionResult<AdminDashboardDto>> GetAdminDashboard()
            => Ok(await _service.GetAdminDashboardAsync());
    }
}
