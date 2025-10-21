using BL.Contracts;
using Domains.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortalSystemProject.Models.JobSeeker;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortalSystemProject.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class CVManagementController : Controller
    {
        private readonly ICVFileRepository _cvRepo;
        private readonly IJobSeekerProfileRepository _profileRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public CVManagementController(
            ICVFileRepository cvRepo,
            IJobSeekerProfileRepository profileRepo,
            UserManager<ApplicationUser> userManager)
        {
            _cvRepo = cvRepo;
            _profileRepo = profileRepo;
            _userManager = userManager;
        }

 
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
            {
                TempData["Error"] = "A profile must be created first.";
                return RedirectToAction("CreateProfile", "JobSeeker");
            }

            var cvs = await _cvRepo.GetCVsByJobSeekerIdAsync(profile.Id);

            var viewModel = new CVManagementViewModel
            {
                CVFiles = cvs.Select(cv => new CVItemViewModel
                {
                    Id = cv.Id,
                    FileName = cv.FileName,
                    ContentType = cv.ContentType,
                    BlobUrl = cv.BlobUrl,
                    FileSizeBytes = cv.FileSizeBytes,
                    IsPrimary = cv.IsPrimary,
                    UploadedAt = cv.UploadedAt,
                    //UsedInApplications = _cvRepo.GetCVUsageCountAsync(cv.Id).Result
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public async Task<IActionResult> Upload(IFormFile CVFile, bool SetAsPrimary = false)
        {
            // ✅ Check if file is uploaded
            if (CVFile == null || CVFile.Length == 0)
            {
                TempData["Error"] = "الرجاء اختيار ملف السيرة الذاتية";
                return RedirectToAction(nameof(Index));
            }

            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
            {
                TempData["Error"] = "يجب إنشاء ملف تعريف أولاً";
                return RedirectToAction("CreateProfile", "JobSeeker");
            }

            var (success, message, cvFileId) = await _cvRepo.UploadCVAsync(
                profile.Id,
                CVFile,
                SetAsPrimary);

            if (success)
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPrimary(Guid id)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
            {
                return Json(new { success = false, message = "Profile not found" });
            }

            var success = await _cvRepo.SetPrimaryAsync(id, profile.Id);

            if (success)
            {
                return Json(new { success = true, message = "The CV is set as the primary one." });
            }

            return Json(new { success = false, message = "An error occurred." });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
            {
                return Json(new { success = false, message = "Profile not found" });
            }

            var (success, message) = await _cvRepo.DeleteCVAsync(id, profile.Id);

            return Json(new { success, message });
        }

 
        [HttpGet]
        public async Task<IActionResult> Download(Guid id)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
            {
                return NotFound();
            }

            var cvs = await _cvRepo.GetCVsByJobSeekerIdAsync(profile.Id);
            var cv = cvs.FirstOrDefault(c => c.Id == id);

            if (cv == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cv.BlobUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, cv.ContentType, cv.FileName);
        }
    }
}