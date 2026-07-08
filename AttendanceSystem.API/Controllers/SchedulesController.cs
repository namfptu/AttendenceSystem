using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _service;
        public SchedulesController(IScheduleService service) { _service = service; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleDto>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("ByClassSubject/{classSubjectId}")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetByClassSubject(int classSubjectId)
            => Ok(await _service.GetByClassSubjectIdAsync(classSubjectId));

        [HttpPost]
        public async Task<ActionResult<ScheduleDto>> Create([FromBody] ScheduleDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ScheduleDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
