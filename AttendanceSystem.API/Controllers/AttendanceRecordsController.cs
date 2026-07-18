using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceRecordsController : ControllerBase
    {
        private readonly IAttendanceRecordService _service;
        public AttendanceRecordsController(IAttendanceRecordService service) { _service = service; }

        [HttpGet("Session/{sessionId}")]
        public async Task<ActionResult<IEnumerable<AttendanceRecordDto>>> GetBySession(int sessionId)
            => Ok(await _service.GetBySessionIdAsync(sessionId));

        [HttpPost("TakeAttendance")]
        public async Task<IActionResult> TakeAttendance([FromBody] TakeAttendanceDto dto, [FromQuery] int lecturerId)
            => await _service.SaveAttendanceAsync(dto, lecturerId) ? Ok(new { success = true }) : BadRequest("Failed to save attendance.");

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecord(int id, [FromBody] AttendanceRecordDto dto, [FromQuery] int lecturerId)
            => await _service.UpdateRecordAsync(id, dto, lecturerId) ? Ok() : BadRequest("Failed to update record.");

        [HttpGet("History/{studentId}/{classSubjectId}")]
        public async Task<ActionResult<IEnumerable<AttendanceRecordDto>>> GetHistory(int studentId, int classSubjectId)
            => Ok(await _service.GetStudentHistoryAsync(studentId, classSubjectId));
    }
}
