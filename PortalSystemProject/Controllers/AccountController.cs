using BL.Contracts;
using BL.Dtos.AccountDtos;
using Microsoft.AspNetCore.Mvc;

namespace PortalSystemProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

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

            var result = await _accountService.RegisterAsync(model);
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

        // POST: /Account/Login
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

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Login");
        }
    }
}
