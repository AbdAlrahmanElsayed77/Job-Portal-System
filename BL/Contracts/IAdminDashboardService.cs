using BL.Dtos.Dashboard;
using Domains;
using Domains.UserModel;

namespace BL.Contracts
{
    public interface IAdminDashboardService
    {

        Task<DashboardSummaryDto> GetDashboardSummaryAsync(string period);


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
