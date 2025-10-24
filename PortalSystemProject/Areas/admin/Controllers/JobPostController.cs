using DAL.DbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalSystemProject.Models.Admin;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortalSystemProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class JobPostController : Controller
    {
        private readonly PortalContext _context;

        public JobPostController(PortalContext context)
        {
            _context = context;
        }

        // ===============================
        // GET: Admin/JobPost
        // ===============================
        public async Task<IActionResult> Index(string? status, string? search)
        {
            var query = _context.JobPosts
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Include(j => j.JobType)
                .Include(j => j.Applications)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active")
                    query = query.Where(j => j.IsActive);
                else if (status == "inactive")
                    query = query.Where(j => !j.IsActive);
                else if (status == "expired")
                    query = query.Where(j => j.ExpiresAt.HasValue && j.ExpiresAt < DateTime.Now);
            }

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(j =>
                    j.Title.Contains(search) ||
                    j.Company.Name.Contains(search));
            }

            var jobPosts = await query
                .Select(j => new JobPostItemViewModel
                {
                    Id = j.Id,
                    Title = j.Title,
                    CompanyName = j.Company.Name,
                    CompanyLogoUrl = j.Company.LogoUrl,
                    CategoryName = j.JobCategory.Name,
                    JobTypeName = j.JobType.Name,
                    Location = $"{j.Country}, {j.City}",
                    IsActive = j.IsActive,
                    ApplicationsCount = j.Applications.Count,
                    PostedDate = j.CreatedDate ?? DateTime.Now,
                    ExpiryDate = j.ExpiresAt
                })
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();

            var viewModel = new JobPostListViewModel
            {
                JobPosts = jobPosts,
                TotalCount = jobPosts.Count,
                ActiveCount = await _context.JobPosts.CountAsync(j => j.IsActive),
                InactiveCount = await _context.JobPosts.CountAsync(j => !j.IsActive),
                StatusFilter = status,
                SearchTerm = search
            };

            return View(viewModel);
        }

        // ===============================
        // GET: Admin/JobPost/Details/{id}
        // ===============================
        public async Task<IActionResult> Details(Guid id)
        {
            var jobPost = await _context.JobPosts
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Include(j => j.JobType)
                .Include(j => j.Applications)
                .Include(j => j.CreatedByUser)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobPost == null)
                return NotFound();

            var viewModel = new JobPostDetailsViewModel
            {
                Id = jobPost.Id,
                Title = jobPost.Title,
                Description = jobPost.Description,
                Requirements = jobPost.Requirements,

                CompanyId = jobPost.CompanyId,
                CompanyName = jobPost.Company.Name,
                CompanyLogoUrl = jobPost.Company.LogoUrl,
                CompanyWebsite = jobPost.Company.Website,

                CategoryName = jobPost.JobCategory.Name,
                JobTypeName = jobPost.JobType != null ? jobPost.JobType.Name : "N/A",

                Location = $"{jobPost.Country ?? ""}{(string.IsNullOrEmpty(jobPost.City) ? "" : ", " + jobPost.City)}",
                ExpiryDate = jobPost.ExpiresAt,
                IsActive = jobPost.IsActive,
                PostedDate = jobPost.CreatedDate ?? DateTime.Now,
                MinSalary = jobPost.MinSalary,
                MaxSalary = jobPost.MaxSalary,
                TotalApplications = jobPost.Applications.Count,
                PendingApplications = jobPost.Applications.Count(a => a.CurrentState == 0),
                AcceptedApplications = jobPost.Applications.Count(a => a.CurrentState == 1),
                RejectedApplications = jobPost.Applications.Count(a => a.CurrentState == 2),
                PostedBy = jobPost.CreatedByUser?.Email ?? "Unknown"
            };

            return View(viewModel);
        }

        // ===============================
        // POST: Admin/JobPost/ToggleStatus
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var jobPost = await _context.JobPosts.FindAsync(id);
            if (jobPost == null)
            {
                TempData["ErrorMessage"] = "Job post not found.";
                return RedirectToAction(nameof(Index));
            }

            jobPost.IsActive = !jobPost.IsActive;
            jobPost.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Job post {(jobPost.IsActive ? "activated" : "deactivated")} successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // POST: Admin/JobPost/Delete
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var jobPost = await _context.JobPosts
                .Include(j => j.Applications)
                .Include(j => j.SavedBy)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jobPost == null)
            {
                TempData["ErrorMessage"] = "Job post not found.";
                return RedirectToAction(nameof(Index));
            }

            if (jobPost.Applications.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete a job post that has existing applications.";
                return RedirectToAction(nameof(Index));
            }

            // 🧹 Delete related SavedJobs first
            if (jobPost.SavedBy.Any())
                _context.SavedJobs.RemoveRange(jobPost.SavedBy);

            _context.JobPosts.Remove(jobPost);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Job post deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}