using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using AttendanceSystem.Web.Services;
using AttendanceSystem.Business.DTOs;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;

namespace AttendanceSystem.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IApiClient _apiClient;

        public ProfileController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Auth");

            var user = await _apiClient.GetAsync<UserDto>($"Users/{userIdStr}");
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string? email, string? phone, IFormFile? avatarFile)
        {
            var userIdStr = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Auth");

            string? avatarUrl = null;

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(avatarFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                avatarUrl = "/uploads/avatars/" + uniqueFileName;
            }

            var updateDto = new UpdateProfileDto
            {
                Email = email,
                Phone = phone,
                AvatarUrl = avatarUrl
            };

            await _apiClient.PutAsync($"Users/{userIdStr}", updateDto);

            // Cập nhật lại Claims
            var identity = (ClaimsIdentity)User.Identity;
            
            if (avatarUrl != null)
            {
                var existingClaim = identity.FindFirst("AvatarUrl");
                if (existingClaim != null) identity.RemoveClaim(existingClaim);
                identity.AddClaim(new Claim("AvatarUrl", avatarUrl));
            }
            
            if (email != null)
            {
                var existingEmailClaim = identity.FindFirst(ClaimTypes.Email);
                if (existingEmailClaim != null) identity.RemoveClaim(existingEmailClaim);
                identity.AddClaim(new Claim(ClaimTypes.Email, email));
            }

            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = true });

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
