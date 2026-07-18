using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubjectsController : Controller
    {
        private readonly IApiClient _apiClient;

        public SubjectsController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            var subjects = await _apiClient.GetAsync<IEnumerable<SubjectDto>>("Subjects") ?? new List<SubjectDto>();
            return View(subjects);
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubjectCode,SubjectName,Credits,TotalSlots,Description")] SubjectDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiClient.PostRawAsync("Subjects", model);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", errorMsg ?? "Failed to create subject. Code might be duplicated.");
            }
            return View(model);
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _apiClient.GetAsync<SubjectDto>($"Subjects/{id}");
            
            if (subject == null) return NotFound();

            return View(subject);
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectCode,SubjectName,Credits,TotalSlots,Description")] SubjectDto model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var response = await _apiClient.PutRawAsync($"Subjects/{id}", model);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", errorMsg ?? "Failed to update subject. Code might be duplicated.");
            }
            return View(model);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _apiClient.DeleteRawAsync($"Subjects/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            var errorMsg = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = errorMsg ?? "Failed to delete subject.";
            return RedirectToAction(nameof(Index));
        }
    }
}
