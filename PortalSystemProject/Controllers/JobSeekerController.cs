using BL.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortalSystemProject.Models.JobSeeker;
using Domains.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalSystemProject.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class JobSeekerController : Controller
    {
        private readonly IJobPostRepository _jobPostRepo;
        private readonly IJobCategoryRepository _categoryRepo;
        private readonly IJobTypeRepository _jobTypeRepo;
        private readonly ICompanyRepository _companyRepo;           
        private readonly ICVFileRepository _cvRepo;
        private readonly IApplicationRepository _applicationRepo;
        private readonly ISavedJobRepository _savedJobRepo;
        private readonly IJobSeekerProfileRepository _profileRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobSeekerController(
            IJobPostRepository jobPostRepo,
            IJobCategoryRepository categoryRepo,
            IJobTypeRepository jobTypeRepo,
            ICompanyRepository companyRepo,                         
            ICVFileRepository cvRepo,
            IApplicationRepository applicationRepo,
            ISavedJobRepository savedJobRepo,
            IJobSeekerProfileRepository profileRepo,
            UserManager<ApplicationUser> userManager)
        {
            _jobPostRepo = jobPostRepo;
            _categoryRepo = categoryRepo;
            _jobTypeRepo = jobTypeRepo;
            _companyRepo = companyRepo;                            
            _cvRepo = cvRepo;
            _applicationRepo = applicationRepo;
            _savedJobRepo = savedJobRepo;
            _profileRepo = profileRepo;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> BrowseJobs(
            string? search,
            Guid? categoryId,
            Guid? jobTypeId,
            string? country,
            string? city,
            decimal? minSalary,
            decimal? maxSalary,
            byte? minExp,
            byte? maxExp,
            string sortBy = "recent",
            int page = 1)
        {
            var pageSize = 10;

            var allCategories = await _categoryRepo.GetAllCategoriesAsync();
            var allJobTypes = await _jobTypeRepo.GetAllJobTypesAsync();
            var allCompanies = _companyRepo.GetAll();               
            var companyDictionary = allCompanies.ToDictionary(c => c.Id, c => c);

            var (jobs, totalCount) = await _jobPostRepo.GetFilteredJobsAsync(
                search, categoryId, jobTypeId, country, city,
                minSalary, maxSalary, minExp, maxExp, sortBy, page, pageSize);

            var categoriesWithCount = await _categoryRepo.GetCategoriesWithJobCountAsync();
            var jobTypesWithCount = await _jobTypeRepo.GetJobTypesWithJobCountAsync();
            var countries = await _jobPostRepo.GetAvailableCountriesAsync();
            var cities = string.IsNullOrEmpty(country)
                ? new List<string>()
                : await _jobPostRepo.GetAvailableCitiesAsync(country);

            List<Guid> savedJobIds = new();
            List<Guid> appliedJobIds = new();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = Guid.Parse(_userManager.GetUserId(User)!);
                var profile = await _profileRepo.GetByUserIdAsync(userId);

                if (profile != null)
                {
                    savedJobIds = await _savedJobRepo.GetSavedJobIdsAsync(profile.Id);
                    var (applications, _) = await _applicationRepo.GetJobSeekerApplicationsAsync(
                        profile.Id, page: 1, pageSize: 1000);
                    appliedJobIds = applications.Select(a => a.JobPostId).ToList();
                }
            }

            var viewModel = new JobListViewModel
            {
                Jobs = jobs.Select(j => new JobCardViewModel
                {
                    Id = j.Id,
                    Title = j.Title,

                    CompanyName = companyDictionary.GetValueOrDefault(j.CompanyId,
                        new BL.Dtos.CompanyDto { Name = "Unknown Company" }).Name,
                    CompanyLogo = companyDictionary.GetValueOrDefault(j.CompanyId,
                        new BL.Dtos.CompanyDto { LogoUrl = null }).LogoUrl,

                    City = j.City,
                    Country = j.Country,

                    JobCategory = allCategories.FirstOrDefault(c => c.Id == j.JobCategoryId)?.Name ?? "Unknown Category",
                    JobType = j.JobTypeId.HasValue ?
                        allJobTypes.FirstOrDefault(t => t.Id == j.JobTypeId.Value)?.Name : null,

                    SalaryRange = FormatSalaryRange(j.MinSalary, j.MaxSalary, j.Currency),
                    ExperienceRange = FormatExperienceRange(j.MinExperienceYears, j.MaxExperienceYears),
                    PublishedAt = j.PublishedAt ?? DateTime.Now,
                    IsSaved = savedJobIds.Contains(j.Id),
                    HasApplied = appliedJobIds.Contains(j.Id)
                }).ToList(),

                Categories = categoriesWithCount.Select(c => new CategoryFilterItem
                {
                    Id = c.Category.Id,
                    Name = c.Category.Name,
                    JobCount = c.JobCount
                }).ToList(),

                JobTypes = jobTypesWithCount.Select(t => new JobTypeFilterItem
                {
                    Id = t.JobType.Id,
                    Name = t.JobType.Name,
                    JobCount = t.JobCount
                }).ToList(),

                Countries = countries,
                Cities = cities,

                Filters = new JobFilterModel
                {
                    SearchKeyword = search,
                    CategoryId = categoryId,
                    JobTypeId = jobTypeId,
                    Country = country,
                    City = city,
                    MinSalary = minSalary,
                    MaxSalary = maxSalary,
                    MinExperience = minExp,
                    MaxExperience = maxExp,
                    SortBy = sortBy
                },

                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalJobs = totalCount,
                PageSize = pageSize,

                AllCategories = allCategories,
                AllJobTypes = allJobTypes,
                CompanyDictionary = companyDictionary
            };

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> JobDetails(Guid id)
        {
            var job = await _jobPostRepo.GetJobDetailsWithCompanyAsync(id);
            if (job == null)
                return NotFound();

            var allCategories = await _categoryRepo.GetAllCategoriesAsync();
            var allJobTypes = await _jobTypeRepo.GetAllJobTypesAsync();
            var allCompanies = _companyRepo.GetAll();              
            var companyDictionary = allCompanies.ToDictionary(c => c.Id, c => c);

            var totalApplications = await _jobPostRepo.GetApplicationsCountAsync(id);

            bool isSaved = false;
            bool hasApplied = false;
            bool canApply = true;
            Guid? currentApplicationId = null;
            var availableCVs = new List<CVOption>();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = Guid.Parse(_userManager.GetUserId(User)!);
                var profile = await _profileRepo.GetByUserIdAsync(userId);

                if (profile != null)
                {
                    isSaved = await _savedJobRepo.IsJobSavedAsync(id, profile.Id);
                    hasApplied = await _applicationRepo.HasAppliedAsync(id, profile.Id);
                    currentApplicationId = await _applicationRepo.GetApplicationIdAsync(id, profile.Id);
                    canApply = !hasApplied && job.IsActive;

                    var cvs = await _cvRepo.GetCVsByJobSeekerIdAsync(profile.Id);
                    availableCVs = cvs.Select(cv => new CVOption
                    {
                        Id = cv.Id,
                        FileName = cv.FileName,
                        IsPrimary = cv.IsPrimary,
                        UploadedAt = cv.UploadedAt
                    }).ToList();
                }
            }

            var viewModel = new JobDetailsViewModel
            {
                JobId = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                CompanyId = job.CompanyId,

                CompanyName = companyDictionary.GetValueOrDefault(job.CompanyId,
                    new BL.Dtos.CompanyDto { Name = "Unknown Company" }).Name,
                CompanyLogo = companyDictionary.GetValueOrDefault(job.CompanyId,
                    new BL.Dtos.CompanyDto { LogoUrl = null }).LogoUrl,
                CompanyWebsite = companyDictionary.GetValueOrDefault(job.CompanyId,
                    new BL.Dtos.CompanyDto { Website = null }).Website,
                CompanyDescription = companyDictionary.GetValueOrDefault(job.CompanyId,
                    new BL.Dtos.CompanyDto { Description = null }).Description,

                Category = allCategories.FirstOrDefault(c => c.Id == job.JobCategoryId)?.Name ?? "Unknown Category",
                JobType = job.JobTypeId.HasValue ?
                    allJobTypes.FirstOrDefault(t => t.Id == job.JobTypeId.Value)?.Name : null,

                Location = $"{job.City}, {job.Country}",
                ExperienceRequired = FormatExperienceRange(job.MinExperienceYears, job.MaxExperienceYears),
                SalaryRange = FormatSalaryRange(job.MinSalary, job.MaxSalary, job.Currency),
                PublishedAt = job.PublishedAt ?? DateTime.Now,
                ExpiresAt = job.ExpiresAt,
                IsSaved = isSaved,
                HasApplied = hasApplied,
                CanApply = canApply,
                CurrentApplicationId = currentApplicationId,
                AvailableCVs = availableCVs,
                TotalApplications = totalApplications
            };

            return View(viewModel);
        }

        // private string GetCompanyName(Guid companyId) => "Company Name"; 
        // private string? GetCompanyLogo(Guid companyId) => null;
        // private string? GetCompanyWebsite(Guid companyId) => null;
        // private string? GetCompanyDescription(Guid companyId) => null;
        // private string GetCategoryName(Guid categoryId) => "Category";
        // private string? GetJobTypeName(Guid jobTypeId) => "Job Type";

        private string? FormatSalaryRange(decimal? min, decimal? max, string? currency)
        {
            if (!min.HasValue && !max.HasValue) return null;
            if (min.HasValue && max.HasValue)
                return $"{min:N0} - {max:N0} {currency ?? "EGP"}";
            if (min.HasValue)
                return $"From {min:N0} {currency ?? "EGP"}";
            return $"To {max:N0} {currency ?? "EGP"}";
        }

        private string? FormatExperienceRange(byte? min, byte? max)
        {
            if (!min.HasValue && !max.HasValue) return null;
            if (min.HasValue && max.HasValue)
                return $"{min} - {max} Years";
            if (min.HasValue)
                return $"From {min} Years";
            return $"To {max} Years";
        }
    }
}