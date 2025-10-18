using BL.Contracts;
using BL.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace PortalSystem.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployerProfileController : Controller
    {
        private readonly IEmployerProfileService _profileService;

        public EmployerProfileController(IEmployerProfileService profileService)
        {
            _profileService = profileService;
        }

        public IActionResult Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = _profileService.GetByUserId(userId);

            if (profile == null)
                return RedirectToAction("Edit");

            return View(profile);
        }

        [HttpGet]
        public IActionResult Edit(Guid? id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = id.HasValue
                ? _profileService.GetById(id.Value, p=>p.Company)
                : new EmployerProfileDto { UserId = userId, CreatedAt = DateTime.UtcNow };

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployerProfileDto model, IFormFile? logoFile)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!ModelState.IsValid)
                return View(model);
            
            var success = _profileService.SaveProfile(model, userId, logoFile);

            if (!success)
                ModelState.AddModelError("", "Error saving profile");

            return RedirectToAction("Index");
        }
    }
}
