using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AttendanceSystem.Web.Services;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize(Roles = "Admin,Lecturer")]
    public class ExportController : Controller
    {
        private readonly IApiClient _apiClient;
        public ExportController(IApiClient apiClient) { _apiClient = apiClient; }

        public async Task<IActionResult> Attendance(int classSubjectId)
        {
            var httpClient = new System.Net.Http.HttpClient { BaseAddress = new System.Uri("http://localhost:5056/api/") };
            var response = await httpClient.GetAsync($"Export/Attendance/{classSubjectId}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Attendance_{classSubjectId}.xlsx");
        }
    }
}
