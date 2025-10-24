using DAL.DbContext;
using Domains;
using Domains.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PortalContext _context;

        public UserManagementController(UserManager<ApplicationUser> userManager, PortalContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Admin/UserManagement
        public async Task<IActionResult> Index(string? role, string? search)
        {
            var usersQuery = _userManager.Users
                .Include(u => u.EmployerProfile)
                    .ThenInclude(ep => ep.Company)
                .Include(u => u.JobSeekerProfile)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                usersQuery = usersQuery.Where(u =>
                    u.Email.Contains(search) ||
                    (u.FName + " " + u.LName).Contains(search));
            }

            var usersList = await usersQuery.ToListAsync();
            var model = new UserManagementViewModel();

            foreach (var user in usersList)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Filter by role (if provided)
                if (!string.IsNullOrEmpty(role) && !roles.Contains(role))
                    continue;

                // Determine company name if employer
                var companyName = user.EmployerProfile?.Company?.Name;

                model.Users.Add(new UserItemViewModel
                {
                    Id = user.Id,
                    FullName = $"{user.FName} {user.LName}",
                    Email = user.Email ?? "",
                    EmailConfirmed = user.EmailConfirmed,
                    IsActive = user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.Now,
                    Roles = roles.ToList(),
                    CompanyName = companyName,
                    ApplicationsCount = await _context.Applications.CountAsync(a => a.ApplicantUserId == user.Id),
                    JobPostsCount = await _context.JobPosts.CountAsync(j => j.CreatedByUserId == user.Id),
                });
            }

            model.TotalCount = model.Users.Count;
            model.JobSeekersCount = model.Users.Count(u => u.Roles.Contains("JobSeeker"));
            model.EmployersCount = model.Users.Count(u => u.Roles.Contains("Employer"));
            model.AdminsCount = model.Users.Count(u => u.Roles.Contains("Admin"));
            model.RoleFilter = role;
            model.SearchTerm = search;

            return View(model);
        }

        // POST: Admin/UserManagement/LockUser/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            user.LockoutEnd = DateTimeOffset.MaxValue;
            await _userManager.UpdateAsync(user);

            return Json(new { success = true, message = "User locked successfully" });
        }

        // POST: Admin/UserManagement/UnlockUser/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            return Json(new { success = true, message = "User unlocked successfully" });
        }

        // GET: Admin/UserManagement/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _userManager.Users
                .Include(u => u.EmployerProfile)
                    .ThenInclude(ep => ep.Company)
                .Include(u => u.JobSeekerProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var viewModel = new UserItemViewModel
            {
                Id = user.Id,
                FullName = $"{user.FName} {user.LName}",
                Email = user.Email ?? "",
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList(),
                CompanyName = user.EmployerProfile?.Company?.Name,
                ApplicationsCount = await _context.Applications.CountAsync(a => a.ApplicantUserId == user.Id),
                JobPostsCount = await _context.JobPosts.CountAsync(j => j.CreatedByUserId == user.Id),
                IsActive = user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.Now
            };

            return View(viewModel);
        }
    }
}
