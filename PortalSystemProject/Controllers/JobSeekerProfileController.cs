using BL.Contracts;
using BL.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "JobSeeker")]
public class JobSeekerProfileController : Controller
{
    private readonly IJobSeekerProfileService _profileService;

    public JobSeekerProfileController(IJobSeekerProfileService profileService)
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

            // ✅ Get profile with CVFiles included
            profile = _profileService.GetById(profile.Id, p => p.CVFiles);

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

            JobSeekerProfileDto model;

            if (id.HasValue && id.Value != Guid.Empty)
            {
                model = _profileService.GetById(id.Value);
                if (model == null)
                {
                    TempData["Error"] = "Profile not found";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Check if user already has a profile
                model = _profileService.GetByUserId(userId);
                if (model == null || model.Id == Guid.Empty)
                {
                    model = new JobSeekerProfileDto
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                }
            }

            return View(model);
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error: " + ex.Message;
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(JobSeekerProfileDto model, IFormFile? photoFile, IFormFile? cvFile)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            _profileService.SaveProfile(model, userId, photoFile, cvFile);
            TempData["Success"] = "Profile saved successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error saving profile: " + ex.Message;
            return View(model);
        }
    }
}

// ============================================
// EmployerProfileController.cs
// ============================================
