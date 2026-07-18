using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceSessionsController : ControllerBase
    {
        private readonly IAttendanceSessionService _service;
        public AttendanceSessionsController(IAttendanceSessionService service) { _service = service; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceSessionDto>>> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<AttendanceSessionDto>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("Lecturer/{lecturerId}")]
        public async Task<ActionResult<IEnumerable<AttendanceSessionDto>>> GetByLecturer(int lecturerId)
            => Ok(await _service.GetByLecturerIdAsync(lecturerId));

        [HttpGet("Today/{lecturerId}")]
        public async Task<ActionResult<IEnumerable<AttendanceSessionDto>>> GetToday(int lecturerId)
            => Ok(await _service.GetTodaySessionsByLecturerAsync(lecturerId));

        [HttpPost]
        public async Task<ActionResult<AttendanceSessionDto>> Create([FromBody] AttendanceSessionDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/Open")]
        public async Task<IActionResult> Open(int id, [FromQuery] bool isAdmin = false)
        {
            var session = await _service.GetByIdAsync(id);
            if (session == null) return NotFound();
            
            var now = System.DateTime.Now;
            var start = session.SessionDate.Date.Add(session.StartTime);
            var end = session.SessionDate.Date.Add(session.EndTime);
            
            if (!isAdmin && (now < start.AddMinutes(-30) || now > end)) 
            {
                return BadRequest($"Hệ thống chỉ cho phép mở phiên điểm danh trong khoảng {start.AddMinutes(-30):HH:mm} đến {end:HH:mm}.");
            }
            
            return await _service.OpenSessionAsync(id) ? Ok() : BadRequest("Cannot open session.");
        }

        [HttpPut("{id}/Close")]
        public async Task<IActionResult> Close(int id)
            => await _service.CloseSessionAsync(id) ? Ok() : BadRequest("Cannot close session.");
    }
}
