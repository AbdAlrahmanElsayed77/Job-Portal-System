using BL.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IJobTypeRepository
    {

        Task<List<(JobTypeDto JobType, int JobCount)>> GetJobTypesWithJobCountAsync();

        Task<List<JobTypeDto>> GetAllJobTypesAsync();
    }
}