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
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var profile = _profileService.GetByUserId(userId);

                if (profile == null || profile.Id == Guid.Empty)
                    return RedirectToAction("Edit");

                return View(profile);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading profile: " + ex.Message;
                return RedirectToAction("Edit");
            }
        }

        [HttpGet]
        public IActionResult Edit(Guid? id)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                EmployerProfileDto profile;

                if (id.HasValue && id.Value != Guid.Empty)
                {
                    profile = _profileService.GetById(id.Value, p => p.Company);
                    if (profile == null)
                    {
                        TempData["Error"] = "Profile not found";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    // Check if user already has a profile
                    profile = _profileService.GetByUserId(userId);
                    if (profile == null || profile.Id == Guid.Empty)
                    {
                        profile = new EmployerProfileDto
                        {
                            UserId = userId,
                            CreatedAt = DateTime.UtcNow
                        };
                    }
                }

                return View(profile);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EmployerProfileDto model, IFormFile? logoFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var success = _profileService.SaveProfile(model, userId, logoFile);

                if (!success)
                {
                    ModelState.AddModelError("", "Error saving profile");
                    return View(model);
                }

                TempData["Success"] = "Profile saved successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error saving profile: " + ex.Message;
                return View(model);
            }
        }
    }
}