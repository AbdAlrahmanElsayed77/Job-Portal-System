using Domains;
using Domains.UserModel;

namespace DAL.Contracts
{
    public interface IAdminDashboardRepository
    {
        // -------- Dashboard Summary --------
        Task<int> GetUserCountAsync();
        Task<int> GetCompanyCountAsync();
        Task<int> GetJobCountAsync(DateTime? fromDate = null);
        Task<int> GetCategoryCountAsync();
        Task<(List<string> labels, List<int> counts)> GetJobsByCategoryAsync(DateTime? fromDate = null);
        Task<(List<string> labels, List<int> counts)> GetJobsByCompanyAsync(DateTime? fromDate = null);

        // -------- Management --------
        Task<List<EmployerProfile>> GetEmployersAsync();
        Task<List<JobPost>> GetJobsAsync();
        Task<JobPost?> GetJobByIdAsync(Guid id);
        Task DeleteJobAsync(Guid id);
        Task<List<ApplicationUser>> GetUsersAsync();
        Task<List<Company>> GetCompaniesAsync();
        Task<List<JobSeekerProfile>> GetJobSeekersAsync();
        Task<List<JobCategory>> GetIndustriesAsync();
        Task<List<JobPost>> GetJobsByCategoryAsync(Guid categoryId);
        Task<List<Company>> GetCompaniesByCategoryAsync(Guid categoryId);
    }
}
