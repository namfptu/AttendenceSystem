using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Business.Services;

namespace AttendanceSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _service;
        public ExportController(IExportService service) { _service = service; }

        [HttpGet("Attendance/{classSubjectId}")]
        public async Task<IActionResult> ExportAttendance(int classSubjectId)
        {
            var bytes = await _service.ExportAttendanceByClassSubjectAsync(classSubjectId);
            if (bytes == null) return NotFound();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Attendance_{classSubjectId}.xlsx");
        }
    }
}
