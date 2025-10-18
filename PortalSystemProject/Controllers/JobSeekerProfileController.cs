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
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var profile = _profileService.GetByUserId(userId);
        profile = _profileService.GetById(profile.Id,p=>p.CVFiles);
        if (profile == null)
            return RedirectToAction("Edit");
        return View(profile);
    }

    [HttpGet]
    public IActionResult Edit(Guid? id)
    {
        var model = _profileService.GetOrCreate(id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(JobSeekerProfileDto model, IFormFile? photoFile, IFormFile? cvFile)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        _profileService.SaveProfile(model, userId, photoFile, cvFile);

        return RedirectToAction(nameof(Index));
    }
}
