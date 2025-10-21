using BL.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface ICVFileRepository
    {
        Task<List<CVFileDto>> GetCVsByJobSeekerIdAsync(Guid jobSeekerId);
        Task<(bool Success, string Message, Guid? CVFileId)> UploadCVAsync(Guid jobSeekerId, IFormFile file, bool setAsPrimary = false);
        Task<(bool Success, string Message)> DeleteCVAsync(Guid cvFileId, Guid jobSeekerId);
        Task<bool> SetPrimaryAsync(Guid cvFileId, Guid jobSeekerId);
        Task<List<CVFileDto>> GetCVsByIdsAsync(List<Guid> ids);
        Task<CVFileDto?> GetCVByIdAsync(Guid id);
        Task<CVFileDto?> GetPrimaryCVAsync(Guid jobSeekerId);
        Task<int> GetCVUsageCountAsync(Guid cvFileId);
    }
}