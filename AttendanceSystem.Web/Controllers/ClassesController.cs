using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClassesController : Controller
    {
        private readonly IApiClient _apiClient;

        public ClassesController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Classes
        public async Task<IActionResult> Index()
        {
            var classes = await _apiClient.GetAsync<IEnumerable<ClassDto>>("Classes") ?? new List<ClassDto>();
            return View(classes);
        }

        // GET: Classes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Classes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassCode,ClassName")] ClassDto model)
        {
            if (ModelState.IsValid)
            {
                var created = await _apiClient.PostAsync<ClassDto, ClassDto>("Classes", model);
                if (created != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                
                ModelState.AddModelError("", "Failed to create class. Code might be duplicated.");
            }
            return View(model);
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var classObj = await _apiClient.GetAsync<ClassDto>($"Classes/{id}");
            
            if (classObj == null) return NotFound();

            return View(classObj);
        }

        // POST: Classes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassCode,ClassName")] ClassDto model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await _apiClient.PutAsync($"Classes/{id}", model);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to update class. Code might be duplicated.");
            }
            return View(model);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiClient.DeleteAsync($"Classes/{id}");
            return RedirectToAction(nameof(Index));
        }

        // GET: Classes/Students/5
        public async Task<IActionResult> Students(int id)
        {
            var classObj = await _apiClient.GetAsync<ClassDto>($"Classes/{id}");
            if (classObj == null) return NotFound();

            ViewBag.Class = classObj;

            var classStudents = await _apiClient.GetAsync<IEnumerable<ClassStudentDto>>($"ClassStudents/Class/{id}") ?? new List<ClassStudentDto>();
            
            // Get all students for the dropdown
            var allStudents = await _apiClient.GetAsync<IEnumerable<StudentDto>>("Students") ?? new List<StudentDto>();
            ViewBag.AllStudents = allStudents;

            return View(classStudents);
        }

        // POST: Classes/AddStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(int classId, List<int> studentIds)
        {
            if (studentIds == null || studentIds.Count == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một sinh viên.";
                return RedirectToAction(nameof(Students), new { id = classId });
            }

            int successCount = 0;
            var errors = new List<string>();

            foreach (var studentId in studentIds)
            {
                var dto = new ClassStudentDto { ClassId = classId, StudentId = studentId };
                var response = await _apiClient.PostRawAsync("ClassStudents", dto);
                if (response.IsSuccessStatusCode)
                {
                    successCount++;
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    errors.Add(string.IsNullOrEmpty(errorMsg) ? $"Sinh viên ID {studentId} bị lỗi hệ thống." : errorMsg);
                }
            }

            if (errors.Any())
            {
                TempData["ErrorMessage"] = $"Đã thêm thành công {successCount} sinh viên. Thất bại:<br/>" + string.Join("<br/>", errors);
            }
            else
            {
                TempData["SuccessMessage"] = $"Đã thêm thành công {successCount} sinh viên vào lớp.";
            }

            return RedirectToAction(nameof(Students), new { id = classId });
        }

        // POST: Classes/RemoveStudent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudent(int id, int classId)
        {
            await _apiClient.DeleteAsync($"ClassStudents/{id}");
            return RedirectToAction(nameof(Students), new { id = classId });
        }

        // POST: Classes/ImportStudents
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportStudents(int classId, Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file Excel.";
                return RedirectToAction(nameof(Students), new { id = classId });
            }

            using var content = new System.Net.Http.MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            using var streamContent = new System.Net.Http.StreamContent(stream);
            content.Add(streamContent, "file", file.FileName);

            var result = await _apiClient.PostMultipartAsync<ImportResultDto>($"ClassStudents/Class/{classId}/Import", content);

            if (result != null)
            {
                if (result.ErrorCount == 0)
                {
                    TempData["SuccessMessage"] = $"Import thành công {result.SuccessCount} sinh viên.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Import {result.SuccessCount} thành công. Lỗi {result.ErrorCount} dòng: " + string.Join(" | ", result.Errors);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Lỗi hệ thống khi gọi API Import.";
            }

            return RedirectToAction(nameof(Students), new { id = classId });
        }
    }
}
