using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LecturersController : Controller
    {
        private readonly IApiClient _apiClient;

        public LecturersController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Lecturers
        public async Task<IActionResult> Index()
        {
            var lecturers = await _apiClient.GetAsync<IEnumerable<LecturerDto>>("Lecturers") ?? new List<LecturerDto>();
            return View(lecturers);
        }

        // GET: Lecturers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lecturers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LecturerCode,Department,FullName,Email")] LecturerDto model)
        {
            if (ModelState.IsValid)
            {
                var created = await _apiClient.PostAsync<LecturerDto, LecturerDto>("Lecturers", model);
                if (created != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                
                // If failed, we should theoretically parse error messages from API, but keeping it simple for now
                ModelState.AddModelError("", "Failed to create lecturer. Code or Email might be duplicated.");
            }
            return View(model);
        }

        // GET: Lecturers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lecturer = await _apiClient.GetAsync<LecturerDto>($"Lecturers/{id}");
            
            if (lecturer == null) return NotFound();

            return View(lecturer);
        }

        // POST: Lecturers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LecturerCode,Department,FullName,Email")] LecturerDto model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await _apiClient.PutAsync($"Lecturers/{id}", model);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to update lecturer. Code or Email might be duplicated.");
            }
            return View(model);
        }

        // POST: Lecturers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiClient.DeleteAsync($"Lecturers/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
