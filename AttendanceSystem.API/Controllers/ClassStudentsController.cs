using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassStudentsController : ControllerBase
    {
        private readonly IClassStudentService _classStudentService;

        public ClassStudentsController(IClassStudentService classStudentService)
        {
            _classStudentService = classStudentService;
        }

        // GET: api/ClassStudents/Class/5
        [HttpGet("Class/{classId}")]
        public async Task<ActionResult<IEnumerable<ClassStudentDto>>> GetStudentsByClass(int classId)
        {
            var students = await _classStudentService.GetStudentsByClassAsync(classId);
            return Ok(students);
        }

        // POST: api/ClassStudents
        [HttpPost]
        public async Task<ActionResult<ClassStudentDto>> PostClassStudent([FromBody] ClassStudentDto dto)
        {
            if (await _classStudentService.ExistsAsync(dto.ClassId, dto.StudentId))
            {
                return BadRequest("Student is already in this class.");
            }

            var createdDto = await _classStudentService.AddStudentToClassAsync(dto.ClassId, dto.StudentId);
            return Ok(createdDto);
        }

        // DELETE: api/ClassStudents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClassStudent(int id)
        {
            var result = await _classStudentService.RemoveStudentFromClassAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/ClassStudents/Class/5/Import
        [HttpPost("Class/{classId}/Import")]
        public async Task<ActionResult<ImportResultDto>> ImportStudents(int classId, Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            using var stream = file.OpenReadStream();
            var result = await _classStudentService.ImportStudentsAsync(classId, stream);
            return Ok(result);
        }
    }
}
