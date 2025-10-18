using BL.Dtos;
using Domains;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IJobTypeRepository: IBaseServices<JobType, JobTypeDto>
    {

        Task<List<(JobTypeDto JobType, int JobCount)>> GetJobTypesWithJobCountAsync();

        Task<List<JobTypeDto>> GetAllJobTypesAsync();
    }
}