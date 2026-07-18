using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize(Roles = "Admin,Lecturer")]
    public class AttendanceSessionsController : Controller
    {
        private readonly IApiClient _apiClient;
        public AttendanceSessionsController(IApiClient apiClient) { _apiClient = apiClient; }

        public async Task<IActionResult> Index()
        {
            IEnumerable<AttendanceSessionDto> sessions;
            if (User.IsInRole("Admin"))
            {
                sessions = await _apiClient.GetAsync<IEnumerable<AttendanceSessionDto>>("AttendanceSessions") ?? new List<AttendanceSessionDto>();
            }
            else
            {
                // Get lecturer ID from claims
                var lecturerIdStr = User.FindFirstValue("LecturerId");
                if (string.IsNullOrEmpty(lecturerIdStr)) return Forbid();
                sessions = await _apiClient.GetAsync<IEnumerable<AttendanceSessionDto>>($"AttendanceSessions/Lecturer/{lecturerIdStr}") ?? new List<AttendanceSessionDto>();
            }
            return View(sessions);
        }

        public async Task<IActionResult> Create()
        {
            var classSubjects = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>("ClassSubjects") ?? new List<ClassSubjectDto>();
            ViewBag.ClassSubjects = classSubjects;
            return View(new AttendanceSessionDto { SessionDate = DateTime.Now, LateAfterMinutes = 15 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendanceSessionDto model)
        {
            if (ModelState.IsValid)
            {
                var lecturerIdStr = User.FindFirstValue("LecturerId");
                if (!string.IsNullOrEmpty(lecturerIdStr))
                    model.CreatedByLecturerId = int.Parse(lecturerIdStr);

                var res = await _apiClient.PostRawAsync("AttendanceSessions", model);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                var err = await res.Content.ReadAsStringAsync();
                ModelState.AddModelError("", string.IsNullOrEmpty(err) ? "Failed to create session." : err);
            }
            var classSubjects = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>("ClassSubjects") ?? new List<ClassSubjectDto>();
            ViewBag.ClassSubjects = classSubjects;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAndOpen(AttendanceSessionDto model)
        {
            // Trích xuất LecturerId
            var lecturerIdStr = User.FindFirstValue("LecturerId");
            if (!string.IsNullOrEmpty(lecturerIdStr))
                model.CreatedByLecturerId = int.Parse(lecturerIdStr);

            // Gọi API tạo Session (trả về id)
            var created = await _apiClient.PostAsync<AttendanceSessionDto, AttendanceSessionDto>("AttendanceSessions", model);
            
            if (created != null && created.Id > 0)
            {
                bool isAdmin = User.IsInRole("Admin");
                var res = await _apiClient.PutRawAsync($"AttendanceSessions/{created.Id}/Open?isAdmin={isAdmin}", new { });
                if (res.IsSuccessStatusCode)
                {
                    // Redirect thẳng tới màn hình Take Attendance
                    return RedirectToAction("Index", "TakeAttendance", new { sessionId = created.Id });
                }
                else
                {
                    // Lỗi validation (ví dụ: mở quá sớm)
                    var err = await res.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = err;
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            
            TempData["ErrorMessage"] = "Failed to create session.";
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Open(int id)
        {
            bool isAdmin = User.IsInRole("Admin");
            var res = await _apiClient.PutRawAsync($"AttendanceSessions/{id}/Open?isAdmin={isAdmin}", new { });
            if (!res.IsSuccessStatusCode)
            {
                var err = await res.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = string.IsNullOrEmpty(err) ? "Cannot open session." : err;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            await _apiClient.PutAsync($"AttendanceSessions/{id}/Close", new { });
            return RedirectToAction(nameof(Index));
        }
    }
}
