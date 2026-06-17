using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SemestersController : Controller
    {
        private readonly IApiClient _apiClient;

        public SemestersController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Semesters
        public async Task<IActionResult> Index()
        {
            var semesters = await _apiClient.GetAsync<IEnumerable<SemesterDto>>("Semesters") ?? new List<SemesterDto>();
            return View(semesters);
        }

        // GET: Semesters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Semesters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,StartDate,EndDate")] SemesterDto model)
        {
            if (ModelState.IsValid)
            {
                var created = await _apiClient.PostAsync<SemesterDto, SemesterDto>("Semesters", model);
                if (created != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                
                ModelState.AddModelError("", "Failed to create semester. Name might be duplicated.");
            }
            return View(model);
        }

        // GET: Semesters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var semester = await _apiClient.GetAsync<SemesterDto>($"Semesters/{id}");
            
            if (semester == null) return NotFound();

            return View(semester);
        }

        // POST: Semesters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate")] SemesterDto model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await _apiClient.PutAsync($"Semesters/{id}", model);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to update semester. Name might be duplicated.");
            }
            return View(model);
        }

        // POST: Semesters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiClient.DeleteAsync($"Semesters/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
