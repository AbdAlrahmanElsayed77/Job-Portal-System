using BL.Contracts;
using DAL.DbContext;
using Domains.UserModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BL.Services
{
    public class AdminAnalyticsService : IAdminAnalyticsService
    {
        private readonly PortalContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminAnalyticsService(PortalContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<int> GetTotalJobSeekersCountAsync()
        {
            return await _context.JobSeekerProfiles.CountAsync();
        }

        public async Task<int> GetTotalEmployersCountAsync()
        {
            return await _context.EmployerProfiles.CountAsync();
        }

        public async Task<int> GetTotalAdminsCountAsync()
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            return adminUsers.Count;
        }

        public async Task<int> GetTotalJobsCountAsync()
        {
            return await _context.JobPosts.CountAsync();
        }

        public async Task<int> GetActiveJobsCountAsync()
        {
            return await _context.JobPosts.CountAsync(j => j.IsActive);
        }

        public async Task<int> GetInactiveJobsCountAsync()
        {
            return await _context.JobPosts.CountAsync(j => !j.IsActive);
        }

        public async Task<int> GetTotalApplicationsCountAsync()
        {
            return await _context.Applications.CountAsync();
        }

        public async Task<int> GetPendingApplicationsCountAsync()
        {
            return await _context.Applications.CountAsync(a => a.CurrentState == 0);
        }

        public async Task<int> GetTotalCompaniesCountAsync()
        {
            return await _context.Companies.CountAsync();
        }

        public async Task<int> GetPendingCompaniesCountAsync()
        {
            return await _context.Companies.CountAsync(c => c.Status == 0);
        }

        public async Task<int> GetApprovedCompaniesCountAsync()
        {
            return await _context.Companies.CountAsync(c => c.Status == 1);
        }

        public async Task<Dictionary<string, int>> GetJobsByCategoryAsync()
        {
            return await _context.JobPosts
                .Include(j => j.JobCategory)
                .GroupBy(j => j.JobCategory.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetApplicationsByMonthAsync(int months = 6)
        {
            var startDate = DateTime.Now.AddMonths(-months);

            var applications = await _context.Applications
                .Where(a => a.CreatedDate >= startDate)
                .GroupBy(a => new {
                    Year = a.CreatedDate!.Value.Year,
                    Month = a.CreatedDate.Value.Month
                })
                .Select(g => new {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            return applications.ToDictionary(
                x => $"{x.Year}-{x.Month:D2}",
                x => x.Count
            );
        }
    }
}