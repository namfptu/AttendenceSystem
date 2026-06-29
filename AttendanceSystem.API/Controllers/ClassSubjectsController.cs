using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassSubjectsController : ControllerBase
    {
        private readonly IClassSubjectService _classSubjectService;

        public ClassSubjectsController(IClassSubjectService classSubjectService)
        {
            _classSubjectService = classSubjectService;
        }

        // GET: api/ClassSubjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassSubjectDto>>> GetClassSubjects([FromQuery] int? semesterId)
        {
            var results = await _classSubjectService.GetAllAsync(semesterId);
            return Ok(results);
        }

        // GET: api/ClassSubjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClassSubjectDto>> GetClassSubject(int id)
        {
            var result = await _classSubjectService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/ClassSubjects
        [HttpPost]
        public async Task<ActionResult<ClassSubjectDto>> PostClassSubject([FromBody] ClassSubjectDto dto)
        {
            if (await _classSubjectService.ExistsAsync(dto.ClassId, dto.SubjectId, dto.SemesterId))
            {
                return BadRequest("This class is already assigned to this subject in the given semester.");
            }

            var createdDto = await _classSubjectService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetClassSubject), new { id = createdDto.Id }, createdDto);
        }

        // PUT: api/ClassSubjects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClassSubject(int id, [FromBody] ClassSubjectDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (await _classSubjectService.ExistsAsync(dto.ClassId, dto.SubjectId, dto.SemesterId, id))
            {
                return BadRequest("This class is already assigned to this subject in the given semester.");
            }

            var updatedDto = await _classSubjectService.UpdateAsync(id, dto);
            if (updatedDto == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/ClassSubjects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassSubject(int id)
        {
            var result = await _classSubjectService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
