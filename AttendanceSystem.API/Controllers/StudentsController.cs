using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
        {
            var students = await _studentService.GetAllAsync();
            return Ok(students);
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await _studentService.GetByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // POST: api/Students
        [HttpPost]
        public async Task<ActionResult<StudentDto>> PostStudent([FromBody] StudentDto studentDto)
        {
            if (await _studentService.ExistsByEmailAsync(studentDto.Email))
            {
                return BadRequest("Email is already taken.");
            }
            if (await _studentService.ExistsByCodeAsync(studentDto.StudentCode))
            {
                return BadRequest("Student Code is already taken.");
            }

            var createdDto = await _studentService.CreateAsync(studentDto);
            return CreatedAtAction(nameof(GetStudent), new { id = createdDto.Id }, createdDto);
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, [FromBody] StudentDto studentDto)
        {
            if (id != studentDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (await _studentService.ExistsByEmailAsync(studentDto.Email, id))
            {
                return BadRequest("Email is already taken.");
            }
            if (await _studentService.ExistsByCodeAsync(studentDto.StudentCode, id))
            {
                return BadRequest("Student Code is already taken.");
            }

            var updatedDto = await _studentService.UpdateAsync(id, studentDto);
            if (updatedDto == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _studentService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Students/Import
        [HttpPost("Import")]
        public async Task<ActionResult<ImportResultDto>> Import(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không được trống.");

            using var stream = file.OpenReadStream();
            var result = await _studentService.ImportStudentsAsync(stream);
            
            return Ok(result);
        }
    }
}
