using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.Contracts;
using DAL.DbContext;
using Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BL.Services
{
    public class JobPostService :BaseService<JobPost, JobPostDto>, IJobPostRepository
    {
        private readonly PortalContext _context;
        private readonly IMapper _mapper;
        public ITableRepository<JobPost> Repo { get; }


        public JobPostService(ITableRepository<JobPost> repo, PortalContext context, IMapper mapper): base(repo, mapper)
        {
            _context = context;
            _mapper = mapper;
            Repo = repo;
    }


        public async Task<(List<JobPostDto> Jobs, int TotalCount)> GetFilteredJobsAsync(
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
            int pageSize = 10)
        {
            var query = _context.JobPosts
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Include(j => j.JobType)
                .Where(j => j.IsActive);

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.ToLower();
                query = query.Where(j =>
                    j.Title.ToLower().Contains(searchKeyword) ||
                    j.Description.ToLower().Contains(searchKeyword) ||
                    j.Company.Name.ToLower().Contains(searchKeyword));
            }

            if (categoryId.HasValue)
                query = query.Where(j => j.JobCategoryId == categoryId.Value);

            if (jobTypeId.HasValue)
                query = query.Where(j => j.JobTypeId == jobTypeId.Value);

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(j => j.Country == country);

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(j => j.City == city);

            if (minSalary.HasValue)
                query = query.Where(j => j.MinSalary >= minSalary.Value || j.MaxSalary >= minSalary.Value);

            if (maxSalary.HasValue)
                query = query.Where(j => j.MinSalary <= maxSalary.Value);

            if (minExperience.HasValue)
                query = query.Where(j => j.MaxExperienceYears >= minExperience.Value);

            if (maxExperience.HasValue)
                query = query.Where(j => j.MinExperienceYears <= maxExperience.Value);

            query = sortBy.ToLower() switch
            {
                "salary" => query.OrderByDescending(j => j.MaxSalary),
                "experience" => query.OrderBy(j => j.MinExperienceYears),
                "title" => query.OrderBy(j => j.Title),
                _ => query.OrderByDescending(j => j.CreatedDate) 
            };

            var totalCount = await query.CountAsync();

            var jobs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var jobDtos = _mapper.Map<List<JobPostDto>>(jobs);

            return (jobDtos, totalCount);
        }

        public async Task<JobPostDto?> GetJobDetailsWithCompanyAsync(Guid jobPostId)
        {
            var job = await _context.JobPosts
                .Include(j => j.Company)
                .Include(j => j.JobCategory)
                .Include(j => j.JobType)
                .FirstOrDefaultAsync(j => j.Id == jobPostId);

            return job == null ? null : _mapper.Map<JobPostDto>(job);
        }


        public async Task<List<string>> GetAvailableCountriesAsync()
        {
            return await _context.JobPosts
                .Where(j => j.IsActive && !string.IsNullOrEmpty(j.Country))
                .Select(j => j.Country!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }


        public async Task<List<string>> GetAvailableCitiesAsync(string country)
        {
            return await _context.JobPosts
                .Where(j => j.IsActive && j.Country == country && !string.IsNullOrEmpty(j.City))
                .Select(j => j.City!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }


        public async Task<bool> IsJobActiveAsync(Guid jobPostId)
        {
            return await _context.JobPosts
                .AnyAsync(j => j.Id == jobPostId && j.IsActive);
        }


        public async Task<int> GetApplicationsCountAsync(Guid jobPostId)
        {
            return await _context.Applications
                .CountAsync(a => a.JobPostId == jobPostId);
        }


        public List<JobPostDto> getJopsForEmployer(Guid empId)
        {
            return _mapper.Map<List<JobPostDto>>(Repo.GetAll().Where(j => j.CreatedByUserId == empId).ToList());
        }
    }
}