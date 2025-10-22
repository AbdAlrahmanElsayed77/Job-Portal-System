using DAL.Contracts;
using DAL.DbContext;
using Domains;
using Domains.UserModel;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly PortalContext _context;

        public AdminDashboardRepository(PortalContext context)
        {
            _context = context;
        }

        // ---------------- Dashboard Summary ----------------
        public async Task<int> GetUserCountAsync()
            => await _context.Users.CountAsync();

        public async Task<int> GetCompanyCountAsync()
            => await _context.Companies.CountAsync();

        public async Task<int> GetJobCountAsync(DateTime? fromDate = null)
        {
            var query = _context.JobPosts.AsQueryable();
            if (fromDate.HasValue)
                query = query.Where(j => j.CreatedDate >= fromDate.Value);
            return await query.CountAsync();
        }

        public async Task<int> GetCategoryCountAsync()
            => await _context.JobCategories.CountAsync();

        public async Task<(List<string> labels, List<int> counts)> GetJobsByCategoryAsync(DateTime? fromDate = null)
        {
            var jobs = _context.JobPosts.AsQueryable();
            if (fromDate.HasValue)
                jobs = jobs.Where(j => j.CreatedDate >= fromDate.Value);

            var categories = await _context.JobCategories.ToListAsync();
            var labels = categories.Select(c => c.Name).ToList();
            var counts = categories.Select(c => jobs.Count(j => j.JobCategoryId == c.Id)).ToList();

            return (labels, counts);
        }

        public async Task<(List<string> labels, List<int> counts)> GetJobsByCompanyAsync(DateTime? fromDate = null)
        {
            var jobs = _context.JobPosts.AsQueryable();
            if (fromDate.HasValue)
                jobs = jobs.Where(j => j.CreatedDate >= fromDate.Value);

            var companies = await _context.Companies.Take(8).ToListAsync();
            var labels = companies.Select(c => c.Name).ToList();
            var counts = companies.Select(c => jobs.Count(j => j.CompanyId == c.Id)).ToList();

            return (labels, counts);
        }

        // ---------------- Management ----------------
        public async Task<List<EmployerProfile>> GetEmployersAsync()
            => await _context.EmployerProfiles
                .Include(e => e.User)
                .Include(e => e.Company)
                .ToListAsync();

        public async Task<List<JobPost>> GetJobsAsync()
            => await _context.JobPosts
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Include(j => j.JobType)
                .Include(j => j.Applications)
                .ToListAsync();

        public async Task<JobPost?> GetJobByIdAsync(Guid id)
            => await _context.JobPosts
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Include(j => j.JobType)
                .FirstOrDefaultAsync(j => j.Id == id);

        public async Task DeleteJobAsync(Guid id)
        {
            var job = await _context.JobPosts.FindAsync(id);
            if (job != null)
            {
                _context.JobPosts.Remove(job);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ApplicationUser>> GetUsersAsync()
            => await _context.Users
                .Include(u => u.EmployerProfile)
                .Include(u => u.CreatedJobPosts)
                .Include(u => u.Applications)
                .ToListAsync();

        public async Task<List<Company>> GetCompaniesAsync()
            => await _context.Companies
                .Include(c => c.JobPosts)
                .Include(c => c.EmployerProfiles)
                .ToListAsync();

        public async Task<List<JobSeekerProfile>> GetJobSeekersAsync()
            => await _context.JobSeekerProfiles
                .Include(j => j.Applications)
                .Include(j => j.CVFiles)
                .Include(j => j.SavedJobs)
                .ToListAsync();

        public async Task<List<JobCategory>> GetIndustriesAsync()
            => await _context.JobCategories
                .Include(c => c.JobPosts)
                .ThenInclude(p => p.Company)
                .ToListAsync();

        public async Task<List<JobPost>> GetJobsByCategoryAsync(Guid categoryId)
            => await _context.JobPosts
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Where(j => j.JobCategoryId == categoryId)
                .ToListAsync();

        public async Task<List<Company>> GetCompaniesByCategoryAsync(Guid categoryId)
            => await _context.Companies
                .Include(c => c.JobPosts)
                .Include(c => c.EmployerProfiles)
                //.Where(c => c.JobCategoryId == categoryId)
                .ToListAsync();
    }
}
