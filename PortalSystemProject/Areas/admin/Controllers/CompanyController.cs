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
    public class CompanyController : Controller
    {
        private readonly PortalContext _context;

        public CompanyController(PortalContext context)
        {
            _context = context;
        }

        // GET: Admin/Company
        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.Companies.AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "pending")
                    query = query.Where(c => c.Status == 0);
                else if (status == "approved")
                    query = query.Where(c => c.Status == 1);
            }

            var companies = await query
                .Select(c => new CompanyItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Website = c.Website,
                    Country = c.Country,
                    City = c.City,
                    LogoUrl = c.LogoUrl,
                    Status = c.Status,
                    JobPostsCount = c.JobPosts.Count,
                    CreatedAt = c.CreatedDate ?? DateTime.Now
                })
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var viewModel = new CompanyListViewModel
            {
                Companies = companies,
                TotalCount = companies.Count,
                PendingCount = await _context.Companies.CountAsync(c => c.Status == 0),
                ApprovedCount = await _context.Companies.CountAsync(c => c.Status == 1),
                StatusFilter = status
            };

            return View(viewModel);
        }

        // GET: Admin/Company/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var company = await _context.Companies
                .Include(c => c.EmployerProfiles)
                    .ThenInclude(ep => ep.User)
                .Include(c => c.JobPosts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            var viewModel = new CompanyDetailsViewModel
            {
                Id = company.Id,
                Name = company.Name,
                Website = company.Website,
                Country = company.Country,
                City = company.City,
                LogoUrl = company.LogoUrl,
                Description = company.Description,
                Status = company.Status,
                CreatedAt = company.CreatedDate ?? DateTime.Now,
                TotalJobPosts = company.JobPosts.Count,
                ActiveJobPosts = company.JobPosts.Count(j => j.IsActive),
                EmployerEmails = company.EmployerProfiles
                    .Select(ep => ep.User.Email ?? "")
                    .Where(e => !string.IsNullOrEmpty(e))
                    .ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/Company/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
        {
            try
            {
                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    return Json(new { success = false, message = "Company not found" });
                }

                company.Status = 1; // Approved
                company.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Company approved successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/Company/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id)
        {
            try
            {
                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    return Json(new { success = false, message = "Company not found" });
                }

                company.Status = 0; // Pending/Rejected
                company.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Company rejected" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/Company/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var company = await _context.Companies
                    .Include(c => c.JobPosts)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (company == null)
                {
                    return Json(new { success = false, message = "Company not found" });
                }

                // Check if company has job posts
                if (company.JobPosts.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "Cannot delete company with existing job posts"
                    });
                }

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Company deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}