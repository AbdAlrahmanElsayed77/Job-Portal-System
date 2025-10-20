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
        private readonly SignInManager<Domains.UserModel.ApplicationUser> _signInManager;
        public AccountController(
            IAccountService accountService,
            UserManager<Domains.UserModel.ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            SignInManager<Domains.UserModel.ApplicationUser> signInManager)
        {
            _accountService = accountService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
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
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Settings()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login");

                var model = new AccountSettingsDto
                {
                    Email = user.Email,
                    FName = user.FName,
                    LName = user.LName
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading settings: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(AccountSettingsDto model)
        {
            if (!ModelState.IsValid)
                return View("Settings", model);

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login");

                user.FName = model.FName;
                user.LName = model.LName;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Profile updated successfully!";
                }
                else
                {
                    TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                }

                return RedirectToAction("Settings");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating profile: " + ex.Message;
                return RedirectToAction("Settings");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(string newEmail)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login");

                if (user.Email == newEmail)
                {
                    TempData["Warning"] = "This is already your current email address.";
                    return RedirectToAction("Settings");
                }

                var existingUser = await _userManager.FindByEmailAsync(newEmail);
                if (existingUser != null)
                {
                    TempData["Error"] = "This email is already in use.";
                    return RedirectToAction("Settings");
                }

                var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
                var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

                if (result.Succeeded)
                {
                    user.UserName = newEmail;
                    await _userManager.UpdateAsync(user);
                    TempData["Success"] = "Email changed successfully! Please verify your new email.";
                }
                else
                {
                    TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                }

                return RedirectToAction("Settings");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error changing email: " + ex.Message;
                return RedirectToAction("Settings");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide all required fields.";
                return RedirectToAction("Settings");
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login");

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["Success"] = "Password changed successfully!";
                }
                else
                {
                    TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                }

                return RedirectToAction("Settings");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error changing password: " + ex.Message;
                return RedirectToAction("Settings");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(string confirmPassword)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login");

                // Verify password before deletion
                var passwordCheck = await _userManager.CheckPasswordAsync(user, confirmPassword);
                if (!passwordCheck)
                {
                    TempData["Error"] = "Incorrect password. Account not deleted.";
                    return RedirectToAction("Settings");
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    TempData["Success"] = "Your account has been deleted.";
                    return RedirectToAction("Index", "Home");
                }

                TempData["Error"] = "Error deleting account.";
                return RedirectToAction("Settings");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting account: " + ex.Message;
                return RedirectToAction("Settings");
            }
        }
        public IActionResult AccessDenied()
        {
            return RedirectToAction("AccessDenied", "Error");
        }

    }
}