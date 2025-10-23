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
    public class ApplicationController : Controller
    {
        private readonly IApplicationRepository _applicationRepo;
        private readonly IJobPostRepository _jobPostRepo;
        private readonly ICVFileRepository _cvRepo;
        private readonly IJobSeekerProfileRepository _profileRepo;
        private readonly ICompanyRepository _companyRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationController(
            IApplicationRepository applicationRepo,
            IJobPostRepository jobPostRepo,
            ICVFileRepository cvRepo,
            IJobSeekerProfileRepository profileRepo,
            ICompanyRepository companyRepo,
            UserManager<ApplicationUser> userManager)
        {
            _applicationRepo = applicationRepo;
            _jobPostRepo = jobPostRepo;
            _cvRepo = cvRepo;
            _profileRepo = profileRepo;
            _companyRepo = companyRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Apply(Guid jobId)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
            {
                TempData["Error"] = "A profile must be created first.";
                return RedirectToAction("index", "JobSeekerprofile");
            }

            var isActive = await _jobPostRepo.IsJobActiveAsync(jobId);
            if (!isActive)
            {
                TempData["Error"] = "The position is not available for application";
                return RedirectToAction("JobDetails", "JobSeeker", new { id = jobId });
            }

            var hasApplied = await _applicationRepo.HasAppliedAsync(jobId, profile.Id);
            if (hasApplied)
            {
                TempData["Error"] = "You have applied for this job before.";
                return RedirectToAction("JobDetails", "JobSeeker", new { id = jobId });
            }

            var job = await _jobPostRepo.GetJobDetailsWithCompanyAsync(jobId);
            if (job == null)
                return NotFound();

            var company = _companyRepo.GetAll().FirstOrDefault(c => c.Id == job.CompanyId);

            var cvs = await _cvRepo.GetCVsByJobSeekerIdAsync(profile.Id);
            if (!cvs.Any())
            {
                TempData["Error"] = "You must upload your CV first";
                return RedirectToAction("Index", "CVManagement");
            }

            var viewModel = new ApplyJobViewModel
            {
                JobPostId = jobId,
                JobTitle = job.Title,
                CompanyName = company?.Name ?? "Unknown Company",
                AvailableCVs = cvs.Select(cv => new CVOption
                {
                    Id = cv.Id,
                    FileName = cv.FileName,
                    IsPrimary = cv.IsPrimary,
                    UploadedAt = cv.UploadedAt,
                    BlobUrl = cv.BlobUrl
                }).ToList(),
                CVFileId = cvs.FirstOrDefault(cv => cv.IsPrimary)?.Id ?? cvs.First().Id
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ApplyJobViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var cvs = await GetCurrentUserCVsAsync();
                model.AvailableCVs = cvs.Select(cv => new CVOption
                {
                    Id = cv.Id,
                    FileName = cv.FileName,
                    IsPrimary = cv.IsPrimary,
                    UploadedAt = cv.UploadedAt
                }).ToList();
                return View(model);
            }

            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
            {
                TempData["Error"] = "Profile not found";
                return RedirectToAction("CreateProfile", "JobSeeker");
            }

            var (success, message, applicationId) = await _applicationRepo.ApplyForJobAsync(
                model.JobPostId,
                profile.Id,
                userId,
                model.CVFileId,
                model.CoverLetter);

            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction("Details", new { id = applicationId });
            }

            TempData["Error"] = message;
            return RedirectToAction("Apply", new { jobId = model.JobPostId });
        }

        [HttpGet]
        public async Task<IActionResult> MyApplications(int? status, DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
            {
                TempData["Error"] = "A profile must be created first.";
                return RedirectToAction("CreateProfile", "JobSeeker");
            }

            var pageSize = 10;
            var (applications, totalCount) = await _applicationRepo.GetJobSeekerApplicationsAsync(
                profile.Id, status, fromDate, toDate, page, pageSize);

            var jobIds = applications.Select(a => a.JobPostId).Distinct().ToList();
            var jobs = await _jobPostRepo.GetJobsByIdsAsync(jobIds);
            var companyIds = jobs.Select(j => j.CompanyId).Distinct().ToList();
            var companies = _companyRepo.GetAll().Where(c => companyIds.Contains(c.Id)).ToDictionary(c => c.Id);

            var cvIds = applications
                .Where(a => a.CVFileId.HasValue && a.CVFileId.Value != Guid.Empty)
                .Select(a => a.CVFileId!.Value)
                .Distinct()
                .ToList();
            var cvs = cvIds.Any() ? await _cvRepo.GetCVsByIdsAsync(cvIds) : new List<BL.Dtos.CVFileDto>();

            var viewModel = new ApplicationsHistoryViewModel
            {
                Applications = applications.Select(a =>
                {
                    var job = jobs.FirstOrDefault(j => j.Id == a.JobPostId);
                    var company = job != null ? companies.GetValueOrDefault(job.CompanyId??new Guid()) : null;
                    var cv = a.CVFileId.HasValue ? cvs.FirstOrDefault(c => c.Id == a.CVFileId.Value) : null;

                    return new ApplicationItemViewModel
                    {
                        ApplicationId = a.Id,
                        JobPostId = a.JobPostId,
                        JobTitle = job?.Title ?? "Unknown Job",
                        CompanyName = company?.Name ?? "Unknown Company",
                        CompanyLogo = company?.LogoUrl,
                        AppliedAt = a.AppliedAt,
                        Status = GetStatusText((byte)a.Status),
                        StatusClass = GetStatusClass((byte)a.Status),
                        CoverLetter = a.CoverLetter,
                        CVFileName = cv?.FileName ?? "CV.pdf"
                    };
                }).ToList(),
                StatusFilter = status?.ToString() ?? "all",
                FromDate = fromDate,
                ToDate = toDate,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalApplications = totalCount
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
                return NotFound();

            var application = await _applicationRepo.GetApplicationDetailsAsync(id, profile.Id);
            if (application == null)
                return NotFound();

            var job = await _jobPostRepo.GetJobDetailsWithCompanyAsync(application.JobPostId);
            var company = job != null ? _companyRepo.GetAll().FirstOrDefault(c => c.Id == job.CompanyId) : null;

            var cv = application.CVFileId.HasValue && application.CVFileId.Value != Guid.Empty
                ? await _cvRepo.GetCVByIdAsync(application.CVFileId.Value)
                : null;

            var viewModel = new ApplicationDetailsViewModel
            {
                ApplicationId = application.Id,
                JobPostId = application.JobPostId,
                JobTitle = job?.Title ?? "Unknown Job",
                JobDescription = job?.Description,
                CompanyName = company?.Name ?? "Unknown Company",
                CompanyLogo = company?.LogoUrl,
                AppliedAt = application.AppliedAt,
                Status = GetStatusText((byte)application.Status),
                CoverLetter = application.CoverLetter,
                CVFileName = cv?.FileName ?? "CV.pdf",
                CVBlobUrl = cv?.BlobUrl,
                ReviewedAt = application.UpdatedAt,   
                FinalDecisionAt = application.UpdatedAt 
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(Guid id)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);

            if (profile == null)
                return Json(new { success = false, message = "Profile not found" });

            var (success, message) = await _applicationRepo.WithdrawApplicationAsync(id, profile.Id);
            return Json(new { success, message });
        }

        private async Task<List<BL.Dtos.CVFileDto>> GetCurrentUserCVsAsync()
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var profile = await _profileRepo.GetByUserIdAsync(userId);
            return profile != null ? await _cvRepo.GetCVsByJobSeekerIdAsync(profile.Id) : new();
        }

        private string GetStatusText(byte status) => status switch
        {
            0 => "Pending Review",
            1 => "Reviewed",
            2 => "Accepted",
            3 => "Rejected",
            _ => "Unknown"
        };

        private string GetStatusClass(byte status) => status switch
        {
            0 => "badge bg-warning",
            1 => "badge bg-info",
            2 => "badge bg-success",
            3 => "badge bg-danger",
            _ => "badge bg-secondary"
        };
    }
}
