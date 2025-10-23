using BL.Contracts;
using Microsoft.AspNetCore.Mvc;
using PortalSystemProject.Models.Home;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortalSystemProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJobPostRepository _jobPostRepo;
        private readonly IJobCategoryRepository _categoryRepo;
        private readonly ICompanyRepository _companyRepo;

        public HomeController(
            IJobPostRepository jobPostRepo,
            IJobCategoryRepository categoryRepo,ICompanyRepository companyRepository)
        {
            _jobPostRepo = jobPostRepo;
            _categoryRepo = categoryRepo;
            _companyRepo = companyRepository;

        }

  
        public async Task<IActionResult> Index()
        {
            var (recentJobs, _) = await _jobPostRepo.GetFilteredJobsAsync(
                sortBy: "recent",
                page: 1,
                pageSize: 6);

            var categories = await _categoryRepo.GetCategoriesWithJobCountAsync();

            var (allJobs, totalJobs) = await _jobPostRepo.GetFilteredJobsAsync(
                page: 1,
                pageSize: 1);


            var viewModel = new HomeViewModel
            {
                RecentJobs = recentJobs.Select(j => new JobCardItem
                {
                    Id = j.Id,
                    Title = j.Title,
                    CompanyName = j.Company.Name ?? "N/A",
                    City = j.City,
                    Country = j.Country,
                    Category = j.JobCategory.Name??"N/A", 
                    JobType = j.JobType.Name ?? "N/A", 
                    LogoUrl = j.Company.LogoUrl,
                    SalaryRange = FormatSalary(j.MinSalary, j.MaxSalary, j.Currency),
                    PublishedAt = j.PublishedAt ?? DateTime.Now
                }).ToList(),

                Categories = categories.Select(c => new CategoryItem
                {
                    Id = c.Category.Id,
                    Name = c.Category.Name,
                    JobCount = c.JobCount
                }).ToList(),

                TotalJobs = totalJobs,
                TotalCompanies =_companyRepo.GetAll().Count(), 
                TotalCategories = categories.Count
            };

            return View(viewModel);
        }

       
        [HttpGet]
        public IActionResult Search(string? keyword, string? location)
        {
            return RedirectToAction("BrowseJobs", "JobSeeker", new
            {
                search = keyword,
                city = location
            });
        }

        private string? FormatSalary(decimal? min, decimal? max, string? currency)
        {
            if (!min.HasValue && !max.HasValue) return null;
            currency = currency ?? "EGP";
            if (min.HasValue && max.HasValue)
                return $"{min:N0} - {max:N0} {currency}";
            if (min.HasValue)
                return $"From {min:N0} {currency}";
            return $"Up to {max:N0} {currency}";
        }
    }
}