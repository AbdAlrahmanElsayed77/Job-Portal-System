using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BL.Services
{
    public class JobTypeService : IJobTypeRepository
    {
        private readonly PortalContext _context;
        private readonly IMapper _mapper;

        public JobTypeService(PortalContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<(JobTypeDto JobType, int JobCount)>> GetJobTypesWithJobCountAsync()
        {
            var jobTypes = await _context.JobTypes
                .Select(t => new
                {
                    JobType = t,
                    JobCount = t.JobPosts.Count(j => j.IsActive)
                })
                .ToListAsync();

            return jobTypes
                .Select(x => (_mapper.Map<JobTypeDto>(x.JobType), x.JobCount))
                .ToList();
        }


        public async Task<List<JobTypeDto>> GetAllJobTypesAsync()
        {
            var jobTypes = await _context.JobTypes.ToListAsync();
            return _mapper.Map<List<JobTypeDto>>(jobTypes);
        }
    }
}