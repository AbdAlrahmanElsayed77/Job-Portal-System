using BL.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface ISavedJobRepository
    {

        Task<(bool Success, string Message)> SaveJobAsync(Guid jobPostId, Guid jobSeekerId);


        Task<(bool Success, string Message)> UnsaveJobAsync(Guid jobPostId, Guid jobSeekerId);


        Task<bool> IsJobSavedAsync(Guid jobPostId, Guid jobSeekerId);


        Task<List<SavedJobDto>> GetSavedJobsAsync(Guid jobSeekerId);

  
        Task<List<Guid>> GetSavedJobIdsAsync(Guid jobSeekerId);
    }
}