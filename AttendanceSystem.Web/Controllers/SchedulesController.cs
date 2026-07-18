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

        public async Task<IActionResult> Index(int? classSubjectId)
        {
            IEnumerable<ScheduleDto> schedules;
            if (classSubjectId.HasValue)
                schedules = await _apiClient.GetAsync<IEnumerable<ScheduleDto>>($"Schedules/ByClassSubject/{classSubjectId}") ?? new List<ScheduleDto>();
            else
                schedules = await _apiClient.GetAsync<IEnumerable<ScheduleDto>>("Schedules") ?? new List<ScheduleDto>();

            var classSubjects = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>("ClassSubjects") ?? new List<ClassSubjectDto>();
            ViewBag.ClassSubjects = classSubjects;
            ViewBag.SelectedClassSubjectId = classSubjectId;
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
                var created = await _apiClient.PostAsync<ScheduleDto, ScheduleDto>("Schedules", model);
                if (created != null) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Failed to create schedule.");
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
                var success = await _apiClient.PutAsync($"Schedules/{id}", model);
                if (success) return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", "Failed to update schedule.");
            }
            var classSubjects = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>("ClassSubjects") ?? new List<ClassSubjectDto>();
            ViewBag.ClassSubjects = classSubjects;
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiClient.DeleteAsync($"Schedules/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
