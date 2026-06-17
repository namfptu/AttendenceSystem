using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentsController : Controller
    {
        private readonly IApiClient _apiClient;

        public StudentsController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = await _apiClient.GetAsync<IEnumerable<StudentDto>>("Students") ?? new List<StudentDto>();
            return View(students);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentCode,Faculty,Major,FullName,Email")] StudentDto model)
        {
            if (ModelState.IsValid)
            {
                var created = await _apiClient.PostAsync<StudentDto, StudentDto>("Students", model);
                if (created != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                
                ModelState.AddModelError("", "Failed to create student. Code or Email might be duplicated.");
            }
            return View(model);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _apiClient.GetAsync<StudentDto>($"Students/{id}");
            
            if (student == null) return NotFound();

            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentCode,Faculty,Major,FullName,Email")] StudentDto model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await _apiClient.PutAsync($"Students/{id}", model);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to update student. Code or Email might be duplicated.");
            }
            return View(model);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiClient.DeleteAsync($"Students/{id}");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Import(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select an Excel file to import.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var stream = file.OpenReadStream();
                using var content = new System.Net.Http.MultipartFormDataContent();
                var streamContent = new System.Net.Http.StreamContent(stream);
                content.Add(streamContent, "file", file.FileName);

                var result = await _apiClient.PostMultipartAsync<AttendanceSystem.Business.DTOs.ImportResultDto>(
                    "Students/Import",
                    content
                );

                if (result != null)
                {
                    TempData["SuccessMessage"] = $"Imported successfully: {result.SuccessCount} students.";
                    if (result.ErrorCount > 0)
                    {
                        TempData["ErrorMessage"] = $"Failed to import {result.ErrorCount} rows. Errors: " + string.Join(" | ", result.Errors);
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "API returned an empty response or failed to process the request.";
                }
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Import failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
