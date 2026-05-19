using LuxeThreads.Data;
using LuxeThreads.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LuxeThreads.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        // ─── LOGIN GET ───
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // ─── LOGIN POST ───
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            // ─── TEMP DEBUG ───
            var allUsers = _db.Users.ToList();
            TempData["Debug"] = $"Email received: '{model.Email}' | Password received: '{model.Password}' | Total users in DB: {allUsers.Count}";

            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View(model);
            }

            var user = _db.Users.FirstOrDefault(u =>
                u.Email.ToLower() == model.Email.Trim().ToLower() &&
                u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync("CookieAuth", principal, authProps);

            if (user.Role == "Admin")
                return RedirectToAction("Index", "Admin");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // ─── REGISTER GET ───
        public IActionResult Register() => View();

        // ─── REGISTER POST ───
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(model);
            }

            if (_db.Users.Any(u => u.Email.ToLower() == model.Email.ToLower()))
            {
                ModelState.AddModelError("", "An account with this email already exists.");
                return View(model);
            }

            var newUser = new AppUser
            {
                FullName = model.FullName,
                Email = model.Email.Trim(),
                Password = model.Password,
                Role = "User",
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            TempData["Success"] = "Account created! Please login.";
            return RedirectToAction("Login");
        }

        // ─── LOGOUT ───
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();
    }
}