using BL.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IApplicationRepository
    {

        Task<(bool Success, string Message, Guid? ApplicationId)> ApplyForJobAsync(
            Guid jobPostId,
            Guid jobSeekerId,
            Guid applicantUserId,
            Guid cvFileId,
            string? coverLetter = null);


        Task<bool> HasAppliedAsync(Guid jobPostId, Guid jobSeekerId);

 
        Task<Guid?> GetApplicationIdAsync(Guid jobPostId, Guid jobSeekerId);


        Task<(List<ApplicationDto> Applications, int TotalCount)> GetJobSeekerApplicationsAsync(
            Guid jobSeekerId,
            int? statusFilter = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10);

        Task<ApplicationDto?> GetApplicationDetailsAsync(Guid applicationId, Guid jobSeekerId);

  
        Task<(bool Success, string Message)> WithdrawApplicationAsync(Guid applicationId, Guid jobSeekerId);
    }
}