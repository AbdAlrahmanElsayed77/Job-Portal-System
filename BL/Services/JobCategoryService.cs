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
    public class JobCategoryService : BaseService<JobCategory, JobCategoryDto>, IJobCategoryRepository
    {
        private readonly ITableRepository<EmployerProfile> repo;
        private readonly PortalContext _context;
        private readonly IMapper _mapper;

        public JobCategoryService(ITableRepository<JobCategory> repo,PortalContext context, IMapper mapper):base(repo, mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<(JobCategoryDto Category, int JobCount)>> GetCategoriesWithJobCountAsync()
        {
            var categories = await _context.JobCategories
                .Select(c => new
                {
                    Category = c,
                    JobCount = c.JobPosts.Count(j => j.IsActive)
                })
                .ToListAsync();

            return categories
                .Select(x => (_mapper.Map<JobCategoryDto>(x.Category), x.JobCount))
                .ToList();
        }


        public async Task<List<JobCategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.JobCategories.ToListAsync();
            return _mapper.Map<List<JobCategoryDto>>(categories);
        }
    }
}