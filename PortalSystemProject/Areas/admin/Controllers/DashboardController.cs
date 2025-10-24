using BL.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalSystemProject.Models.Admin;
using System.Threading.Tasks;

namespace PortalSystemProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IAdminAnalyticsService _analyticsService;
        private readonly IJobCategoryRepository _categoryRepo;
        private readonly IJobTypeRepository _jobTypeRepo;

        public DashboardController(
            IAdminAnalyticsService analyticsService,
            IJobCategoryRepository categoryRepo,
            IJobTypeRepository jobTypeRepo)
        {
            _analyticsService = analyticsService;
            _categoryRepo = categoryRepo;
            _jobTypeRepo = jobTypeRepo;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                // Users
                TotalUsers = await _analyticsService.GetTotalUsersCountAsync(),
                TotalJobSeekers = await _analyticsService.GetTotalJobSeekersCountAsync(),
                TotalEmployers = await _analyticsService.GetTotalEmployersCountAsync(),
                TotalAdmins = await _analyticsService.GetTotalAdminsCountAsync(),

                // Jobs
                TotalJobs = await _analyticsService.GetTotalJobsCountAsync(),
                ActiveJobs = await _analyticsService.GetActiveJobsCountAsync(),
                InactiveJobs = await _analyticsService.GetInactiveJobsCountAsync(),

                // Applications
                TotalApplications = await _analyticsService.GetTotalApplicationsCountAsync(),
                PendingApplications = await _analyticsService.GetPendingApplicationsCountAsync(),

                // Companies
                TotalCompanies = await _analyticsService.GetTotalCompaniesCountAsync(),
                PendingCompanies = await _analyticsService.GetPendingCompaniesCountAsync(),
                ApprovedCompanies = await _analyticsService.GetApprovedCompaniesCountAsync(),

                // Categories & Types
                TotalCategories = (await _categoryRepo.GetAllCategoriesAsync()).Count,
                TotalJobTypes = (await _jobTypeRepo.GetAllJobTypesAsync()).Count,

                // Charts
                JobsByCategory = await _analyticsService.GetJobsByCategoryAsync(),
                ApplicationsByMonth = await _analyticsService.GetApplicationsByMonthAsync()
            };

            return View(viewModel);
        }
    }
}