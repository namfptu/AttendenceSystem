using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Collections.Generic;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClassSubjectsController : Controller
    {
        private readonly IApiClient _apiClient;

        public ClassSubjectsController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: ClassSubjects
        public async Task<IActionResult> Index(int? semesterId)
        {
            var url = semesterId.HasValue ? $"ClassSubjects?semesterId={semesterId.Value}" : "ClassSubjects";
            var data = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>(url) ?? new List<ClassSubjectDto>();

            var semesters = await _apiClient.GetAsync<IEnumerable<SemesterDto>>("Semesters") ?? new List<SemesterDto>();
            ViewBag.Semesters = new SelectList(semesters, "Id", "Name", semesterId);

            return View(data);
        }

        private async Task PopulateDropdowns()
        {
            var classes = await _apiClient.GetAsync<IEnumerable<ClassDto>>("Classes") ?? new List<ClassDto>();
            var subjects = await _apiClient.GetAsync<IEnumerable<SubjectDto>>("Subjects") ?? new List<SubjectDto>();
            var lecturers = await _apiClient.GetAsync<IEnumerable<LecturerDto>>("Lecturers") ?? new List<LecturerDto>();
            var semesters = await _apiClient.GetAsync<IEnumerable<SemesterDto>>("Semesters") ?? new List<SemesterDto>();

            ViewBag.Classes = new SelectList(classes, "Id", "ClassCode");
            ViewBag.Subjects = new SelectList(subjects, "Id", "SubjectName");
            ViewBag.Lecturers = new SelectList(lecturers, "Id", "FullName");
            ViewBag.Semesters = new SelectList(semesters, "Id", "Name");
        }

        // GET: ClassSubjects/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: ClassSubjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassId,SubjectId,LecturerId,SemesterId")] ClassSubjectDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiClient.PostRawAsync("ClassSubjects", model);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", errorMsg ?? "Failed to assign. This assignment may already exist.");
            }
            await PopulateDropdowns();
            return View(model);
        }

        // POST: ClassSubjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _apiClient.DeleteRawAsync($"ClassSubjects/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            
            var errorMsg = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = errorMsg ?? "Failed to delete assignment.";
            return RedirectToAction(nameof(Index));
        }
    }
}
