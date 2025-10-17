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
    [Authorize]
    public class SavedJobsController : Controller
    {
        private readonly ISavedJobRepository _savedJobRepo;
        private readonly IApplicationRepository _applicationRepo;
        private readonly IJobSeekerProfileRepository _profileRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public SavedJobsController(
            ISavedJobRepository savedJobRepo,
            IApplicationRepository applicationRepo,
            IJobSeekerProfileRepository profileRepo,
            UserManager<ApplicationUser> userManager)
        {
            _savedJobRepo = savedJobRepo;
            _applicationRepo = applicationRepo;
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
                TempData["Error"] = "A profile must be created first.اً";
                return RedirectToAction("CreateProfile", "JobSeeker");
            }

            var savedJobs = await _savedJobRepo.GetSavedJobsAsync(profile.Id);

            var (applications, _) = await _applicationRepo.GetJobSeekerApplicationsAsync(
                profile.Id, page: 1, pageSize: 1000);
            var appliedJobIds = applications.Select(a => a.JobPostId).ToList();

            var viewModel = new SavedJobsViewModel
            {
                SavedJobs = savedJobs.Select(s => new SavedJobItemViewModel
                {
                    JobPostId = s.JobPostId,
                    Title = "Job Title", 
                    CompanyName = "Company Name",
                    Location = "Location",
                    Category = "Category",
                    JobType = "Job Type",
                    SalaryRange = "Salary Range",
                    SavedAt = s.SavedAt,
                    PublishedAt = DateTime.Now,
                    HasApplied = appliedJobIds.Contains(s.JobPostId),
                    IsActive = true
                }).ToList(),
                TotalSaved = savedJobs.Count
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Guid jobId)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
            {
                return Json(new { success = false, message = "Profile not found" });
            }

            var (success, message) = await _savedJobRepo.SaveJobAsync(jobId, profile.Id);

            return Json(new { success, message });
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unsave(Guid jobId)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
            {
                return Json(new { success = false, message = "Profile not found" });
            }

            var (success, message) = await _savedJobRepo.UnsaveJobAsync(jobId, profile.Id);

            return Json(new { success, message });
        }

  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(Guid jobId)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
            {
                return Json(new { success = false, message = "Profile not found" });
            }

            var isSaved = await _savedJobRepo.IsJobSavedAsync(jobId, profile.Id);

            if (isSaved)
            {
                var (success, message) = await _savedJobRepo.UnsaveJobAsync(jobId, profile.Id);
                return Json(new { success, message, action = "unsaved" });
            }
            else
            {
                var (success, message) = await _savedJobRepo.SaveJobAsync(jobId, profile.Id);
                return Json(new { success, message, action = "saved" });
            }
        }
    }
}