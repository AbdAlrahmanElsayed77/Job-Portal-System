using BL.Contracts;
using BL.Dtos.AccountDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace PortalSystemProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<Domains.UserModel.ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(
            IAccountService accountService,
            UserManager<Domains.UserModel.ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _accountService = accountService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetOrigin() =>
            $"{_httpContextAccessor?.HttpContext?.Request.Scheme}://{_httpContextAccessor?.HttpContext?.Request.Host}";

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _accountService.RegisterAsync(model, GetOrigin());

                if (result.Contains("successful", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Success"] = result;
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError(string.Empty, result);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during registration: " + ex.Message);
                return View(model);
            }
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _accountService.LoginAsync(model);

                if (result.Contains("successful", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Success"] = "Welcome back!";
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, result);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during login: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
        {
            try
            {
                var result = await _accountService.ConfirmEmailAsync(userId, token);
                ViewBag.Message = result;
                ViewBag.Success = result.Contains("successfully", StringComparison.OrdinalIgnoreCase);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "An error occurred: " + ex.Message;
                ViewBag.Success = false;
                return View();
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                var result = await _accountService.ForgotPasswordAsync(email, GetOrigin());
                TempData["Success"] = result;
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "An error occurred: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string email, string token, string newPassword)
        {
            try
            {
                var result = await _accountService.ResetPasswordAsync(email, token, newPassword);

                if (result.Contains("successful", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Success"] = result;
                    return RedirectToAction("Login");
                }

                ViewBag.Message = result;
                ViewBag.Email = email;
                ViewBag.Token = token;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "An error occurred: " + ex.Message;
                ViewBag.Email = email;
                ViewBag.Token = token;
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Profile or /Account/Profile?userId=xxx
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile(Guid? userId)
        {
            try
            {
                var viewer = await _userManager.GetUserAsync(User);
                if (viewer == null)
                    return RedirectToAction("Login");

                var targetId = userId ?? viewer.Id;
                var result = await _accountService.GetProfile(viewer.Id, targetId);

                if (!result.ProfileExists && !result.IsEditable)
                {
                    TempData["Error"] = "This user hasn't created a profile yet.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.IsEditable = result.IsEditable;
                ViewBag.UserRole = result.UserRole;
                ViewBag.ProfileExists = result.ProfileExists;

                return View(result.ViewPath, result.ProfileData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading profile: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Account/MyProfile - Shortcut to own profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (role == "JobSeeker")
                return RedirectToAction("Index", "JobSeekerProfile");
            else if (role == "Employer")
                return RedirectToAction("Index", "EmployerProfile");

            return RedirectToAction("Index", "Home");
        }
    }
}