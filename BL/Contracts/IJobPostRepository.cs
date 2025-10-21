using BL.Dtos;
using Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IJobPostRepository: IBaseServices<JobPost, JobPostDto>
    {
  
        Task<(List<JobPostDto> Jobs, int TotalCount)> GetFilteredJobsAsync(
            string? searchKeyword = null,
            Guid? categoryId = null,
            Guid? jobTypeId = null,
            string? country = null,
            string? city = null,
            decimal? minSalary = null,
            decimal? maxSalary = null,
            byte? minExperience = null,
            byte? maxExperience = null,
            string sortBy = "recent",
            int page = 1,
            int pageSize = 10);

        Task<JobPostDto?> GetJobDetailsWithCompanyAsync(Guid jobPostId);


        Task<List<string>> GetAvailableCountriesAsync();


        Task<List<string>> GetAvailableCitiesAsync(string country);

        Task<bool> IsJobActiveAsync(Guid jobPostId);

 
        Task<int> GetApplicationsCountAsync(Guid jobPostId);
        public List<JobPostDto> getJopsForEmployer(Guid empId);

        Task<List<JobPostDto>> GetJobsByIdsAsync(List<Guid> ids);

    }
}