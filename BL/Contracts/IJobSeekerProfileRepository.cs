using BL.Dtos;
using System;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IJobSeekerProfileRepository
    {

        Task<JobSeekerProfileDto?> GetByUserIdAsync(Guid userId);


        Task<bool> HasProfileAsync(Guid userId);


        Task<Guid> CreateProfileAsync(JobSeekerProfileDto profileDto);


        Task<bool> UpdateProfileAsync(JobSeekerProfileDto profileDto);
    }
}