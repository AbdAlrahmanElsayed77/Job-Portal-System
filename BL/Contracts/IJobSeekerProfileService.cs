using BL.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IJobSeekerProfileService : IBaseServices<Domains.JobSeekerProfile, Dtos.JobSeekerProfileDto>
    {
        JobSeekerProfileDto GetByUserId(Guid userId);
        void SaveProfile(JobSeekerProfileDto dto, Guid userId, IFormFile? photo, IFormFile? cv);
        JobSeekerProfileDto GetOrCreate(Guid? id);
    }
}
