using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IAdminAnalyticsService
    {
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetTotalJobSeekersCountAsync();
        Task<int> GetTotalEmployersCountAsync();
        Task<int> GetTotalAdminsCountAsync();

        Task<int> GetTotalJobsCountAsync();
        Task<int> GetActiveJobsCountAsync();
        Task<int> GetInactiveJobsCountAsync();

        Task<int> GetTotalApplicationsCountAsync();
        Task<int> GetPendingApplicationsCountAsync();

        Task<int> GetTotalCompaniesCountAsync();
        Task<int> GetPendingCompaniesCountAsync();
        Task<int> GetApprovedCompaniesCountAsync();

        Task<Dictionary<string, int>> GetJobsByCategoryAsync();
        Task<Dictionary<string, int>> GetApplicationsByMonthAsync(int months = 6);
    }
}