using BL.Contracts;
using BL.Dtos;
using DAL.Contracts;
using Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "JobSeeker")]
public class JobSeekerProfileController : Controller
{
    private readonly IJobSeekerProfileService _profileService;
    private readonly ITableRepository<CVFile> _cvRepo;
    private readonly ITableRepository<Application> _applicationRepo;
    private readonly IFileService _fileService;

    public JobSeekerProfileController(
        IJobSeekerProfileService profileService,
        ITableRepository<CVFile> cvRepo,
        ITableRepository<Application> applicationRepo,
        IFileService fileService)
    {
        _profileService = profileService;
        _cvRepo = cvRepo;
        _applicationRepo = applicationRepo;
        _fileService = fileService;
    }

    public IActionResult Index()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = _profileService.GetByUserId(userId);

            if (profile == null || profile.Id == Guid.Empty)
                return RedirectToAction("Edit");

            // Get profile with CVFiles and Applications
            profile = _profileService.GetById(profile.Id, p => p.CVFiles, p => p.Applications);

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

            // ✅ Preserve existing PhotoUrl if no new photo uploaded
            if (photoFile == null && model.Id != Guid.Empty)
            {
                var existingProfile = _profileService.GetById(model.Id);
                if (existingProfile != null)
                {
                    model.PhotoUrl = existingProfile.PhotoUrl;
                }
            }

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

    // ===== CV MANAGEMENT =====

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetPrimaryCV(Guid cvId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = _profileService.GetByUserId(userId);

            if (profile == null)
            {
                TempData["Error"] = "Profile not found";
                return RedirectToAction("Index");
            }

            // Get all CVs for this profile
            var allCVs = _cvRepo.GetAll().Where(c => c.JobSeekerId == profile.Id).ToList();

            // Set all to non-primary
            foreach (var cv in allCVs)
            {
                cv.IsPrimary = false;
            }

            // Set selected as primary
            var selectedCV = allCVs.FirstOrDefault(c => c.Id == cvId);
            if (selectedCV != null)
            {
                selectedCV.IsPrimary = true;
                _cvRepo.Update(selectedCV);

                // Update others
                foreach (var cv in allCVs.Where(c => c.Id != cvId))
                {
                    _cvRepo.Update(cv);
                }

                TempData["Success"] = "Primary CV updated successfully!";
            }
            else
            {
                TempData["Error"] = "CV not found";
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error setting primary CV: " + ex.Message;
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCV(Guid cvId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = _profileService.GetByUserId(userId);

            if (profile == null)
            {
                TempData["Error"] = "Profile not found";
                return RedirectToAction("Index");
            }

            var cv = _cvRepo.GetById(cvId);

            if (cv == null || cv.JobSeekerId != profile.Id)
            {
                TempData["Error"] = "CV not found or you don't have permission";
                return RedirectToAction("Index");
            }

            // Check if it's the only CV
            var allCVs = _cvRepo.GetAll().Where(c => c.JobSeekerId == profile.Id).ToList();

            if (allCVs.Count == 1)
            {
                TempData["Warning"] = "Cannot delete your only CV. Upload a new one first.";
                return RedirectToAction("Index");
            }

            // Delete the file from storage
            if (!string.IsNullOrEmpty(cv.BlobUrl))
            {
                var fileName = Path.GetFileName(cv.BlobUrl);
                await _fileService.RemoveFileAsync("uploads/cvs", fileName);
            }

            // If deleting primary CV, make another one primary
            if (cv.IsPrimary)
            {
                var newPrimary = allCVs.FirstOrDefault(c => c.Id != cvId);
                if (newPrimary != null)
                {
                    newPrimary.IsPrimary = true;
                    _cvRepo.Update(newPrimary);
                }
            }

            // Delete from database
            _cvRepo.Delete(cvId);

            TempData["Success"] = "CV deleted successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error deleting CV: " + ex.Message;
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadCV(IFormFile cvFile)
    {
        try
        {
            if (cvFile == null || cvFile.Length == 0)
            {
                TempData["Error"] = "Please select a CV file";
                return RedirectToAction("Index");
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = _profileService.GetByUserId(userId);

            if (profile == null || profile.Id == Guid.Empty)
            {
                TempData["Error"] = "Please create your profile first";
                return RedirectToAction("Edit");
            }

            // Upload file
            var blobUrl = await _fileService.UploadFileAsync("uploads/cvs", cvFile);

            // Check if this is the first CV
            var existingCVs = _cvRepo.GetAll().Where(c => c.JobSeekerId == profile.Id).ToList();

            var cvEntity = new CVFile
            {
                Id = Guid.NewGuid(),
                JobSeekerId = profile.Id,
                BlobUrl = blobUrl,
                FileName = cvFile.FileName,
                ContentType = cvFile.ContentType,
                FileSizeBytes = (int)cvFile.Length,
                IsPrimary = existingCVs.Count == 0, // First CV is primary
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow,
                CurrentState = 1
            };

            _cvRepo.Add(cvEntity);
            TempData["Success"] = "CV uploaded successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error uploading CV: " + ex.Message;
            return RedirectToAction("Index");
        }
    }
}