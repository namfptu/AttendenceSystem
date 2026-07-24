using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SchedulesController : Controller
    {
        private readonly IApiClient _apiClient;
        public SchedulesController(IApiClient apiClient) { _apiClient = apiClient; }

        public async Task<IActionResult> Index(int? classSubjectId, string? className, string? subjectName, string? lecturerName, string? dayOfWeek)
        {
            IEnumerable<ScheduleDto> schedules;
            if (classSubjectId.HasValue)
                schedules = await _apiClient.GetAsync<IEnumerable<ScheduleDto>>($"Schedules/ByClassSubject/{classSubjectId}") ?? new List<ScheduleDto>();
            else
                schedules = await _apiClient.GetAsync<IEnumerable<ScheduleDto>>("Schedules") ?? new List<ScheduleDto>();

            // Apply filters
            if (!string.IsNullOrEmpty(className))
            {
                schedules = schedules.Where(s => s.ClassName != null && s.ClassName.Contains(className, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(subjectName))
            {
                schedules = schedules.Where(s => s.SubjectName != null && s.SubjectName.Contains(subjectName, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(lecturerName))
            {
                schedules = schedules.Where(s => s.LecturerName != null && s.LecturerName.Contains(lecturerName, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(dayOfWeek))
            {
                if (Enum.TryParse<DayOfWeek>(dayOfWeek, true, out var dow))
                {
                    schedules = schedules.Where(s => s.DayOfWeek == dow);
                }
            }

            var classSubjects = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>("ClassSubjects") ?? new List<ClassSubjectDto>();
            ViewBag.ClassSubjects = classSubjects;
            ViewBag.SelectedClassSubjectId = classSubjectId;
            
            ViewBag.ClassName = className;
            ViewBag.SubjectName = subjectName;
            ViewBag.LecturerName = lecturerName;
            ViewBag.DayOfWeek = dayOfWeek;

            return View(schedules);
        }

        public async Task<IActionResult> Create()
        {
            var classSubjects = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>("ClassSubjects") ?? new List<ClassSubjectDto>();
            ViewBag.ClassSubjects = classSubjects;
            return View(new ScheduleDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScheduleDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiClient.PostRawAsync("Schedules", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Schedule created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", string.IsNullOrEmpty(errorMsg) ? "Failed to create schedule." : errorMsg);
            }
            var classSubjects = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>("ClassSubjects") ?? new List<ClassSubjectDto>();
            ViewBag.ClassSubjects = classSubjects;
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var schedule = await _apiClient.GetAsync<ScheduleDto>($"Schedules/{id}");
            if (schedule == null) return NotFound();
            var classSubjects = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>("ClassSubjects") ?? new List<ClassSubjectDto>();
            ViewBag.ClassSubjects = classSubjects;
            return View(schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ScheduleDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiClient.PutRawAsync($"Schedules/{id}", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Schedule updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", string.IsNullOrEmpty(errorMsg) ? "Failed to update schedule." : errorMsg);
            }
            var classSubjects = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>("ClassSubjects") ?? new List<ClassSubjectDto>();
            ViewBag.ClassSubjects = classSubjects;
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _apiClient.DeleteAsync($"Schedules/{id}");
            if (success)
            {
                TempData["SuccessMessage"] = "Schedule deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
