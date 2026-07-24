using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IApiClient _apiClient;
        public DashboardController(IApiClient apiClient) { _apiClient = apiClient; }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin")) return RedirectToAction(nameof(Admin));
            if (User.IsInRole("Lecturer")) return RedirectToAction(nameof(Lecturer));
            if (User.IsInRole("Student")) return RedirectToAction(nameof(Student));
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var data = await _apiClient.GetAsync<AdminDashboardDto>("Dashboard/Admin");
            return View("AdminDashboard", data);
        }

        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> Lecturer()
        {
            var lecturerIdStr = User.FindFirstValue("LecturerId");
            if (string.IsNullOrEmpty(lecturerIdStr)) return Forbid();
            var data = await _apiClient.GetAsync<LecturerDashboardDto>($"Dashboard/Lecturer/{lecturerIdStr}");
            return View("LecturerDashboard", data);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Student()
        {
            var studentIdStr = User.FindFirstValue("StudentId");
            if (string.IsNullOrEmpty(studentIdStr)) return Forbid();
            var data = await _apiClient.GetAsync<StudentDashboardDto>($"Dashboard/Student/{studentIdStr}");
            return View("StudentDashboard", data);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AttendanceHistory(int classSubjectId)
        {
            var studentIdStr = User.FindFirstValue("StudentId");
            if (string.IsNullOrEmpty(studentIdStr)) return Forbid();

            var studentId = int.Parse(studentIdStr);
            var history = await _apiClient.GetAsync<IEnumerable<AttendanceRecordDto>>($"AttendanceRecords/History/{studentId}/{classSubjectId}")
                ?? new List<AttendanceRecordDto>();

            var classSubject = await _apiClient.GetAsync<ClassSubjectDto>($"ClassSubjects/{classSubjectId}");
            ViewBag.ClassSubject = classSubject;

            return View(history);
        }
    }
}
