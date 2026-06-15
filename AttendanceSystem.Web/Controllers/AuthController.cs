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
        private readonly AppDbContext _context;

        public AuthController()
        {
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
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            // Simple hardcoded admin fallback if DB is empty
            if (username?.ToLower() == "admin" && password == "admin123")
            {
                await SignInUser("admin", "Admin", 1);
                return RedirectToLocal(returnUrl);
            }

            // Normal DB Login - temporarily disabled until Users API is built
            /*
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
            if (user != null && user.PasswordHash == password) 
            {
                await SignInUser(user.Username, user.Role.ToString(), user.Id);
                return RedirectToLocal(returnUrl);
            }
            */

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

        private async Task SignInUser(string username, string role, int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserId", userId.ToString())
            };

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
