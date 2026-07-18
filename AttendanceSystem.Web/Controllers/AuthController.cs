using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AttendanceSystem.Data;
using AttendanceSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly AttendanceSystem.Web.Services.IApiClient _apiClient;

        public AuthController(AttendanceSystem.Web.Services.IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            // Simple hardcoded admin fallback if DB is empty
            if (email?.ToLower() == "admin@fpt.edu.vn" && password == "admin123")
            {
                await SignInUser("admin@fpt.edu.vn", "Admin", 1);
                return RedirectToLocal(returnUrl);
            }

            // Normal DB Login via API
            var loginRequest = new AttendanceSystem.Business.DTOs.LoginRequestDto
            {
                Email = email,
                Password = password
            };

            var user = await _apiClient.PostAsync<AttendanceSystem.Business.DTOs.LoginRequestDto, AttendanceSystem.Business.DTOs.UserDto>("Auth/Login", loginRequest);

            if (user != null)
            {
                await SignInUser(user.Username, user.Role, user.Id, user.LecturerId, user.StudentId, user.AvatarUrl, user.Email);
                return RedirectToLocal(returnUrl);
            }

            ViewData["ErrorMessage"] = "Invalid username or password";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task SignInUser(string username, string role, int userId, int? lecturerId = null, int? studentId = null, string? avatarUrl = null, string? email = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId.ToString())
            };

            if (lecturerId.HasValue)
                claims.Add(new Claim("LecturerId", lecturerId.Value.ToString()));

            if (studentId.HasValue)
                claims.Add(new Claim("StudentId", studentId.Value.ToString()));

            if (!string.IsNullOrEmpty(avatarUrl))
                claims.Add(new Claim("AvatarUrl", avatarUrl));
            
            if (!string.IsNullOrEmpty(email))
                claims.Add(new Claim(ClaimTypes.Email, email));

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
            };

            await HttpContext.SignInAsync(
                "Cookies",
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
