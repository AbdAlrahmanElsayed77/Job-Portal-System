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
    public class SavedJobsController : Controller
    {
        private readonly ISavedJobRepository _savedJobRepo;
        private readonly IApplicationRepository _applicationRepo;
        private readonly IJobSeekerProfileRepository _profileRepo;
        private readonly IJobCategoryRepository _categoryRepo;
        private readonly IJobTypeRepository _jobTypeRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public SavedJobsController(
            ISavedJobRepository savedJobRepo,
            IApplicationRepository applicationRepo,
            IJobSeekerProfileRepository profileRepo,
            IJobCategoryRepository categoryRepo,
            IJobTypeRepository jobTypeRepo,
            UserManager<ApplicationUser> userManager)
        {
            _savedJobRepo = savedJobRepo;
            _applicationRepo = applicationRepo;
            _profileRepo = profileRepo;
            _categoryRepo = categoryRepo;
            _jobTypeRepo = jobTypeRepo;
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

            var savedJobs = await _savedJobRepo.GetSavedJobsWithDetailsAsync(profile.Id);

            var (applications, _) = await _applicationRepo.GetJobSeekerApplicationsAsync(
                profile.Id, page: 1, pageSize: 1000);
            var appliedJobIds = applications.Select(a => a.JobPostId).ToList();

            var viewModel = new SavedJobsViewModel
            {
                SavedJobs = savedJobs
                    .Where(s => s.JobPost != null)
                    .Select(s => new SavedJobItemViewModel
                    {
                        JobPostId = s.JobPostId,
                        Title = s.JobPost!.Title,
                        CompanyName = s.JobPost.Company?.Name ?? "Unknown Company",
                        CompanyLogo = s.JobPost.Company?.LogoUrl,
                        Location = $"{s.JobPost.City}, {s.JobPost.Country}",
                        Category = s.JobPost.JobCategory?.Name ?? "Unknown",
                        JobType = s.JobPost.JobType?.Name,
                        SalaryRange = FormatSalaryRange(s.JobPost.MinSalary, s.JobPost.MaxSalary, s.JobPost.Currency),
                        SavedAt = s.SavedAt,
                        PublishedAt = s.JobPost.PublishedAt ?? DateTime.Now,
                        HasApplied = appliedJobIds.Contains(s.JobPostId),
                        IsActive = s.JobPost.IsActive
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

        private string? FormatSalaryRange(decimal? min, decimal? max, string? currency)
        {
            if (!min.HasValue && !max.HasValue) return null;
            if (min.HasValue && max.HasValue)
                return $"{min:N0} - {max:N0} {currency ?? "EGP"}";
            if (min.HasValue)
                return $"From {min:N0} {currency ?? "EGP"}";
            return $"To {max:N0} {currency ?? "EGP"}";
        }
    }
}