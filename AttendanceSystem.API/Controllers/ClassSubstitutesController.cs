using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.DTOs;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassSubstitutesController : ControllerBase
    {
        private readonly IClassSubstituteService _substituteService;

        public ClassSubstitutesController(IClassSubstituteService substituteService)
        {
            _substituteService = substituteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? classSubjectId)
        {
            var result = await _substituteService.GetAllAsync(classSubjectId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _substituteService.GetByIdAsync(id);
            if (result == null) return NotFound("Không tìm thấy phân công dạy thay.");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassSubstituteDto dto)
        {
            try
            {
                var result = await _substituteService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _substituteService.DeleteAsync(id);
            if (!success) return NotFound("Không tìm thấy phân công dạy thay để xóa.");
            return Ok("Xóa phân công dạy thay thành công.");
        }
    }
}
