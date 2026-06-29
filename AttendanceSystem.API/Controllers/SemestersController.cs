using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemestersController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        // GET: api/Semesters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SemesterDto>>> GetSemesters()
        {
            var semesters = await _semesterService.GetAllAsync();
            return Ok(semesters);
        }

        // GET: api/Semesters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SemesterDto>> GetSemester(int id)
        {
            var semester = await _semesterService.GetByIdAsync(id);

            if (semester == null)
            {
                return NotFound();
            }

            return Ok(semester);
        }

        // POST: api/Semesters
        [HttpPost]
        public async Task<ActionResult<SemesterDto>> PostSemester([FromBody] SemesterDto semesterDto)
        {
            if (await _semesterService.ExistsByNameAsync(semesterDto.Name))
            {
                return BadRequest("Semester Name is already taken.");
            }

            var createdDto = await _semesterService.CreateAsync(semesterDto);
            return CreatedAtAction(nameof(GetSemester), new { id = createdDto.Id }, createdDto);
        }

        // PUT: api/Semesters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSemester(int id, [FromBody] SemesterDto semesterDto)
        {
            if (id != semesterDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (await _semesterService.ExistsByNameAsync(semesterDto.Name, id))
            {
                return BadRequest("Semester Name is already taken.");
            }

            var updatedDto = await _semesterService.UpdateAsync(id, semesterDto);
            if (updatedDto == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Semesters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            var result = await _semesterService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
