using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturersController : ControllerBase
    {
        private readonly ILecturerService _lecturerService;

        public LecturersController(ILecturerService lecturerService)
        {
            _lecturerService = lecturerService;
        }

        // GET: api/Lecturers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LecturerDto>>> GetLecturers()
        {
            var lecturers = await _lecturerService.GetAllAsync();
            return Ok(lecturers);
        }

        // GET: api/Lecturers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LecturerDto>> GetLecturer(int id)
        {
            var lecturer = await _lecturerService.GetByIdAsync(id);

            if (lecturer == null)
            {
                return NotFound();
            }

            return Ok(lecturer);
        }

        // POST: api/Lecturers
        [HttpPost]
        public async Task<ActionResult<LecturerDto>> PostLecturer([FromBody] LecturerDto lecturerDto)
        {
            if (await _lecturerService.ExistsByEmailAsync(lecturerDto.Email))
            {
                return BadRequest("Email is already taken.");
            }
            if (await _lecturerService.ExistsByCodeAsync(lecturerDto.LecturerCode))
            {
                return BadRequest("Lecturer Code is already taken.");
            }

            var createdDto = await _lecturerService.CreateAsync(lecturerDto);
            return CreatedAtAction(nameof(GetLecturer), new { id = createdDto.Id }, createdDto);
        }

        // PUT: api/Lecturers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLecturer(int id, [FromBody] LecturerDto lecturerDto)
        {
            if (id != lecturerDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (await _lecturerService.ExistsByEmailAsync(lecturerDto.Email, id))
            {
                return BadRequest("Email is already taken.");
            }
            if (await _lecturerService.ExistsByCodeAsync(lecturerDto.LecturerCode, id))
            {
                return BadRequest("Lecturer Code is already taken.");
            }

            var updatedDto = await _lecturerService.UpdateAsync(id, lecturerDto);
            if (updatedDto == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Lecturers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLecturer(int id)
        {
            var result = await _lecturerService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
