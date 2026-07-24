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

        public async Task<IActionResult> Index(string? className, string? subjectName, string? lecturerName, string? status, DateTime? date)
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

            // Apply filters
            if (!string.IsNullOrEmpty(className))
            {
                sessions = sessions.Where(s => s.ClassName != null && s.ClassName.Contains(className, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(subjectName))
            {
                sessions = sessions.Where(s => s.SubjectName != null && s.SubjectName.Contains(subjectName, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(lecturerName))
            {
                sessions = sessions.Where(s => s.LecturerName != null && s.LecturerName.Contains(lecturerName, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(status))
            {
                sessions = sessions.Where(s => s.Status != null && s.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }
            if (date.HasValue)
            {
                sessions = sessions.Where(s => s.SessionDate.Date == date.Value.Date);
            }

            ViewBag.ClassName = className;
            ViewBag.SubjectName = subjectName;
            ViewBag.LecturerName = lecturerName;
            ViewBag.Status = status;
            ViewBag.Date = date?.ToString("yyyy-MM-dd");

            return View(sessions);
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
                var res = await _apiClient.PutRawAsync($"AttendanceSessions/{created.Id}/Open", new { });
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
            var res = await _apiClient.PutRawAsync($"AttendanceSessions/{id}/Open", new { });
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
