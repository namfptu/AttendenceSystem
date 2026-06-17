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
        public async Task<IActionResult> Create([Bind("SubjectCode,SubjectName,Credits,Description")] SubjectDto model)
        {
            if (ModelState.IsValid)
            {
                var created = await _apiClient.PostAsync<SubjectDto, SubjectDto>("Subjects", model);
                if (created != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                
                ModelState.AddModelError("", "Failed to create subject. Code might be duplicated.");
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,SubjectCode,SubjectName,Credits,Description")] SubjectDto model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await _apiClient.PutAsync($"Subjects/{id}", model);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to update subject. Code might be duplicated.");
            }
            return View(model);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiClient.DeleteAsync($"Subjects/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
