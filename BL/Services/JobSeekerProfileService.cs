using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.Contracts;
using Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class JobSeekerProfileService : BaseService<JobSeekerProfile, JobSeekerProfileDto>, IJobSeekerProfileRepository
    {
        public JobSeekerProfileService(ITableRepository<JobSeekerProfile> repo, IMapper mapper) : base(repo, mapper)
        {
        }
    }
}
