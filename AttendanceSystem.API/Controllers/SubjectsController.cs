using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjects()
        {
            var subjects = await _subjectService.GetAllAsync();
            return Ok(subjects);
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDto>> GetSubject(int id)
        {
            var subject = await _subjectService.GetByIdAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            return Ok(subject);
        }

        // POST: api/Subjects
        [HttpPost]
        public async Task<ActionResult<SubjectDto>> PostSubject([FromBody] SubjectDto subjectDto)
        {
            if (await _subjectService.ExistsByCodeAsync(subjectDto.SubjectCode))
            {
                return BadRequest("Subject Code is already taken.");
            }

            var createdDto = await _subjectService.CreateAsync(subjectDto);
            return CreatedAtAction(nameof(GetSubject), new { id = createdDto.Id }, createdDto);
        }

        // PUT: api/Subjects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubject(int id, [FromBody] SubjectDto subjectDto)
        {
            if (id != subjectDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (await _subjectService.ExistsByCodeAsync(subjectDto.SubjectCode, id))
            {
                return BadRequest("Subject Code is already taken.");
            }

            var updatedDto = await _subjectService.UpdateAsync(id, subjectDto);
            if (updatedDto == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Subjects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var result = await _subjectService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
