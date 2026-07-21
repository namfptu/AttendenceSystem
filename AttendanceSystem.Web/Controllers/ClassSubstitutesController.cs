using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClassSubstitutesController : Controller
    {
        private readonly IApiClient _apiClient;

        public ClassSubstitutesController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: ClassSubstitutes
        public async Task<IActionResult> Index()
        {
            var data = await _apiClient.GetAsync<IEnumerable<ClassSubstituteDto>>("ClassSubstitutes") 
                       ?? new List<ClassSubstituteDto>();
            return View(data);
        }

        // GET: ClassSubstitutes/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new ClassSubstituteDto { SubstituteDate = DateTime.Today });
        }

        // POST: ClassSubstitutes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassSubstituteDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _apiClient.PostRawAsync("ClassSubstitutes", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Phân công giảng viên dạy thay thành công!";
                    return RedirectToAction(nameof(Index));
                }

                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, string.IsNullOrEmpty(errorMsg) ? "Đã có lỗi xảy ra khi lưu phân công." : errorMsg);
            }

            await PopulateDropdowns();
            return View(model);
        }

        // POST: ClassSubstitutes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _apiClient.DeleteRawAsync($"ClassSubstitutes/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Xóa phân công dạy thay thành công!";
            }
            else
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = string.IsNullOrEmpty(errorMsg) ? "Không thể xóa phân công dạy thay." : errorMsg;
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns()
        {
            var lecturers = await _apiClient.GetAsync<IEnumerable<LecturerDto>>("Lecturers") ?? new List<LecturerDto>();
            var classSubjects = await _apiClient.GetAsync<IEnumerable<ClassSubjectDto>>("ClassSubjects") ?? new List<ClassSubjectDto>();

            var classSubjectItems = classSubjects.Select(cs => new
            {
                Id = cs.Id,
                DisplayText = $"{cs.ClassCode} - {cs.SubjectCode}: {cs.SubjectName} ({cs.LecturerName})"
            }).ToList();

            ViewBag.ClassSubjects = new SelectList(classSubjectItems, "Id", "DisplayText");
            ViewBag.Lecturers = new SelectList(lecturers.Select(l => new 
            {
                Id = l.Id,
                DisplayText = $"{l.LecturerCode} - {l.FullName}"
            }), "Id", "DisplayText");
        }
    }
}
