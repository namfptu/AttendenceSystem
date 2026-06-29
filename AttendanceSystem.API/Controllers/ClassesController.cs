using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassesController(IClassService classService)
        {
            _classService = classService;
        }

        // GET: api/Classes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassDto>>> GetClasses()
        {
            var classes = await _classService.GetAllAsync();
            return Ok(classes);
        }

        // GET: api/Classes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClassDto>> GetClass(int id)
        {
            var classObj = await _classService.GetByIdAsync(id);

            if (classObj == null)
            {
                return NotFound();
            }

            return Ok(classObj);
        }

        // POST: api/Classes
        [HttpPost]
        public async Task<ActionResult<ClassDto>> PostClass([FromBody] ClassDto classDto)
        {
            if (await _classService.ExistsByCodeAsync(classDto.ClassCode))
            {
                return BadRequest("Class Code is already taken.");
            }

            var createdDto = await _classService.CreateAsync(classDto);
            return CreatedAtAction(nameof(GetClass), new { id = createdDto.Id }, createdDto);
        }

        // PUT: api/Classes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClass(int id, [FromBody] ClassDto classDto)
        {
            if (id != classDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (await _classService.ExistsByCodeAsync(classDto.ClassCode, id))
            {
                return BadRequest("Class Code is already taken.");
            }

            var updatedDto = await _classService.UpdateAsync(id, classDto);
            if (updatedDto == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Classes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var result = await _classService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
