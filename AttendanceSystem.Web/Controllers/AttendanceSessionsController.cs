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
                // Try to get LecturerId from claims
                var lecturerIdStr = User.FindFirstValue("LecturerId");
                if (!string.IsNullOrEmpty(lecturerIdStr))
                    model.CreatedByLecturerId = int.Parse(lecturerIdStr);

                var created = await _apiClient.PostAsync<AttendanceSessionDto, AttendanceSessionDto>("AttendanceSessions", model);
                if (created != null) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Failed to create session.");
            }
            var classSubjects = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>("ClassSubjects") ?? new List<ClassSubjectDto>();
            ViewBag.ClassSubjects = classSubjects;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Open(int id)
        {
            await _apiClient.PutAsync($"AttendanceSessions/{id}/Open", new { });
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
