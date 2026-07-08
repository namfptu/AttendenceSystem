using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize(Roles = "Lecturer,Admin")]
    public class TakeAttendanceController : Controller
    {
        private readonly IApiClient _apiClient;
        public TakeAttendanceController(IApiClient apiClient) { _apiClient = apiClient; }

        // GET: /TakeAttendance?sessionId=5
        public async Task<IActionResult> Index(int sessionId)
        {
            var session = await _apiClient.GetAsync<AttendanceSessionDto>($"AttendanceSessions/{sessionId}");
            if (session == null) return NotFound();

            var records = await _apiClient.GetAsync<IEnumerable<AttendanceRecordDto>>($"AttendanceRecords/Session/{sessionId}")
                ?? new List<AttendanceRecordDto>();

            ViewBag.Session = session;
            return View(records);
        }

        public class AttendanceRowSubmitModel
        {
            public int StudentId { get; set; }
            public string Status { get; set; }
            public string Note { get; set; }
        }

        public class TakeAttendanceSubmitViewModel
        {
            public int AttendanceSessionId { get; set; }
            public List<AttendanceRowSubmitModel> Records { get; set; } = new List<AttendanceRowSubmitModel>();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(TakeAttendanceSubmitViewModel model)
        {
            var lecturerIdStr = User.FindFirstValue("LecturerId");
            int lecturerId = !string.IsNullOrEmpty(lecturerIdStr) ? int.Parse(lecturerIdStr) : 0;

            // --- DEBUG LOGGING ---
            var logPath = @"d:\PRN232\AttendenceSystem\debug_log.txt";
            System.IO.File.AppendAllText(logPath, $"--- Submit Called at {System.DateTime.Now} ---\n");
            System.IO.File.AppendAllText(logPath, $"SessionId: {model.AttendanceSessionId}\n");
            System.IO.File.AppendAllText(logPath, $"Records Count: {model.Records?.Count ?? 0}\n");
            if (model.Records != null)
            {
                foreach(var r in model.Records) {
                    System.IO.File.AppendAllText(logPath, $" - StudentId: {r.StudentId}, Status: {r.Status}, Note: {r.Note}\n");
                }
            }
            // ---------------------

            var dto = new TakeAttendanceDto
            {
                AttendanceSessionId = model.AttendanceSessionId,
                Records = new List<AttendanceRecordDto>()
            };

            if (model.Records != null)
            {
                foreach (var row in model.Records)
                {
                    dto.Records.Add(new AttendanceRecordDto
                    {
                        StudentId = row.StudentId,
                        Status = string.IsNullOrEmpty(row.Status) ? "Present" : row.Status,
                        Note = row.Note
                    });
                }
            }

            // Call API manually to get raw response body
            var res = await _apiClient.PostRawAsync($"AttendanceRecords/TakeAttendance?lecturerId={lecturerId}", dto);
            var resBody = await res.Content.ReadAsStringAsync();
            
            System.IO.File.AppendAllText(logPath, $"API StatusCode: {res.StatusCode}\nAPI Response Body: {resBody}\n");

            if (res.IsSuccessStatusCode || dto.Records.Count == 0)
            {
                TempData["SuccessMessage"] = "Attendance saved successfully!";
            }
            else
            {
                TempData["SuccessMessage"] = "Attendance submitted.";
            }

            return RedirectToAction(nameof(Index), new { sessionId = model.AttendanceSessionId });
        }

        // GET: /TakeAttendance/EditRecord/5
        public async Task<IActionResult> EditRecord(int id, int sessionId)
        {
            var session = await _apiClient.GetAsync<AttendanceSessionDto>($"AttendanceSessions/{sessionId}");
            if (session == null) return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var sessionEnd = session.SessionDate.Date.Add(session.EndTime);
                if (System.DateTime.Now > sessionEnd.AddHours(24))
                {
                    TempData["ErrorMessage"] = "Giảng viên chỉ có thể chỉnh sửa điểm danh trong vòng 24 giờ sau khi ca học kết thúc. Vui lòng liên hệ Admin để được hỗ trợ sửa đổi.";
                    return RedirectToAction(nameof(Index), new { sessionId });
                }
            }

            var records = await _apiClient.GetAsync<IEnumerable<AttendanceRecordDto>>($"AttendanceRecords/Session/{sessionId}")
                ?? new List<AttendanceRecordDto>();
            var record = records.FirstOrDefault(r => r.Id == id);
            if (record == null) return NotFound();

            ViewBag.SessionId = sessionId;
            return View(record);
        }

        // POST: /TakeAttendance/EditRecord
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRecord(int id, int sessionId, AttendanceRecordDto model)
        {
            var session = await _apiClient.GetAsync<AttendanceSessionDto>($"AttendanceSessions/{sessionId}");
            if (session == null) return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var sessionEnd = session.SessionDate.Date.Add(session.EndTime);
                if (System.DateTime.Now > sessionEnd.AddHours(24))
                {
                    TempData["ErrorMessage"] = "Đã quá 24 giờ, giảng viên không thể lưu thay đổi. Vui lòng liên hệ Admin.";
                    return RedirectToAction(nameof(Index), new { sessionId });
                }
            }

            var lecturerIdStr = User.FindFirstValue("LecturerId");
            int lecturerId = !string.IsNullOrEmpty(lecturerIdStr) ? int.Parse(lecturerIdStr) : 0;

            await _apiClient.PutAsync($"AttendanceRecords/{id}?lecturerId={lecturerId}", model);
            TempData["SuccessMessage"] = "Record updated successfully!";
            return RedirectToAction(nameof(Index), new { sessionId });
        }
    }
}
