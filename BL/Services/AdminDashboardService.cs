using BL.Contracts;
using BL.Dtos.Dashboard;
using DAL.Contracts;
using Domains;
using Domains.UserModel;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IAdminDashboardRepository _repo;

    public AdminDashboardService(IAdminDashboardRepository repo)
    {
        _repo = repo;
    }

    // ------------------- Dashboard Summary -------------------
    private DateTime? GetStartDate(string period)
    {
        var now = DateTime.UtcNow;
        return period switch
        {
            "week" => now.AddDays(-7),
            "month" => now.AddMonths(-1),
            _ => null
        };
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(string period)
    {
        var fromDate = GetStartDate(period);
        var (catLabels, catCounts) = await _repo.GetJobsByCategoryAsync(fromDate);
        var (compLabels, compCounts) = await _repo.GetJobsByCompanyAsync(fromDate);

        return new DashboardSummaryDto
        {
            UserCount = await _repo.GetUserCountAsync(),
            CompanyCount = await _repo.GetCompanyCountAsync(),
            JobCount = await _repo.GetJobCountAsync(fromDate),
            CategoryCount = await _repo.GetCategoryCountAsync(),
            CategoryLabels = catLabels,
            CategoryJobCounts = catCounts,
            CompanyLabels = compLabels,
            CompanyJobCounts = compCounts
        };
    }



    public async Task<List<EmployerProfile>> GetEmployersAsync()
        => await _repo.GetEmployersAsync();

    public async Task<List<JobPost>> GetJobsAsync()
        => await _repo.GetJobsAsync();

    public async Task<JobPost?> GetJobByIdAsync(Guid id)
        => await _repo.GetJobByIdAsync(id);

    public async Task DeleteJobAsync(Guid id)
        => await _repo.DeleteJobAsync(id);

    public async Task<List<ApplicationUser>> GetUsersAsync()
        => await _repo.GetUsersAsync();

    public async Task<List<Company>> GetCompaniesAsync()
        => await _repo.GetCompaniesAsync();

    public async Task<List<JobSeekerProfile>> GetJobSeekersAsync()
        => await _repo.GetJobSeekersAsync();

    public async Task<List<JobCategory>> GetIndustriesAsync()
        => await _repo.GetIndustriesAsync();

    public async Task<List<JobPost>> GetJobsByCategoryAsync(Guid categoryId)
        => await _repo.GetJobsByCategoryAsync(categoryId);

    public async Task<List<Company>> GetCompaniesByCategoryAsync(Guid categoryId)
        => await _repo.GetCompaniesByCategoryAsync(categoryId);


}

