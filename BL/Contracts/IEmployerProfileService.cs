using BL.Dtos;
using Domains;
using Microsoft.AspNetCore.Http;

namespace BL.Contracts
{
    public interface IEmployerProfileService : IBaseServices<EmployerProfile, EmployerProfileDto>
    {
        EmployerProfileDto GetByUserId(Guid userId);
        bool SaveProfile(EmployerProfileDto dto, Guid userId, IFormFile? logoFile);
    }
}
