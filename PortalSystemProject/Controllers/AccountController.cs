using BL.Contracts;
using BL.Dtos.AccountDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PortalSystemProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<Domains.UserModel.ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
        public AccountController(IAccountService accountService, UserManager<Domains.UserModel.ApplicationUser> userManager)
        {
            _accountService = accountService;
            _userManager = userManager;
        }
        private string GetOrigin() =>
            $"{_httpContextAccessor?.HttpContext?.Request.Scheme}://{_httpContextAccessor?.HttpContext?.Request.Host}";

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.RegisterAsync(model, GetOrigin());
            TempData["Message"] = result;

            if (result.Contains("successfully", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Login");

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.LoginAsync(model);
            if (result.Contains("successful", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("AllJops", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
        {
            var result = await _accountService.ConfirmEmailAsync(userId, token);
            ViewBag.Message = result;
            return View();
        }
        [HttpGet]
        public IActionResult ForgotPassword() => View();
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _accountService.ForgotPasswordAsync(email, GetOrigin());
            ViewBag.Message = result;
            return View();
        }
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string token, string newPassword)
        {
            var result = await _accountService.ResetPasswordAsync(email, token, newPassword);
            ViewBag.Message = result;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public async Task<IActionResult> Profile(Guid? userId)
        {
            var viewer = await _userManager.GetUserAsync(User);
            var targetId = userId ?? viewer.Id;

            var result = await _accountService.GetProfile(viewer.Id, targetId);

            ViewBag.IsEditable = result.IsEditable;
            return View(result.ViewPath, result.ProfileData);
        }
    }
}
