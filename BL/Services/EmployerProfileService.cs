using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.Contracts;
using Domains;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace BL.Services
{
    public class EmployerProfileService : BaseService<EmployerProfile, EmployerProfileDto>, IEmployerProfileService
    {
        private readonly ITableRepository<EmployerProfile> _profileRepo;
        private readonly ITableRepository<Company> _companyRepo;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public EmployerProfileService(
            ITableRepository<EmployerProfile> profileRepo,
            ITableRepository<Company> companyRepo,
            IFileService fileService,
            IMapper mapper
        ) : base(profileRepo, mapper)
        {
            _profileRepo = profileRepo;
            _companyRepo = companyRepo;
            _fileService = fileService;
            _mapper = mapper;
        }

        public EmployerProfileDto GetByUserId(Guid userId)
        {
            var profile = _profileRepo
                .GetAll(e => e.Company)
                .FirstOrDefault(x => x.UserId == userId);

            return _mapper.Map<EmployerProfileDto>(profile);
        }

        public bool SaveProfile(EmployerProfileDto dto, Guid userId, IFormFile? logoFile)
        {
            EmployerProfile? profile;

            if (dto.Id == Guid.Empty)
            {
                // ✅ Create new profile
                profile = _mapper.Map<EmployerProfile>(dto);
                profile.UserId = userId;

                if (logoFile != null)
                {
                    var logoUrl = _fileService.UploadFileAsync("company_logos", logoFile).Result;

                    var company = _companyRepo.GetById(dto.CompanyId);
                    if (company != null)
                    {
                        company.LogoUrl = logoUrl;
                        _companyRepo.Update(company);
                    }
                }

                return _profileRepo.Add(profile);
            }
            else
            {
                // 🟠 Update existing
                profile = _profileRepo.GetById(dto.Id, p => p.Company);
                if (profile == null)
                    throw new Exception("Profile not found");
                dto.CompanyId = profile.CompanyId;
                _mapper.Map(dto, profile);
                profile.UpdatedBy = userId;

                if (logoFile != null)
                {
                    var logoUrl = _fileService.UploadFileAsync("company_logos", logoFile).Result;

                    // ✅ Use the already tracked Company entity
                    if (profile.Company != null)
                    {
                        profile.Company.LogoUrl = logoUrl;
                        _companyRepo.Update(profile.Company);
                    }
                    else
                    {
                        // Fallback if for some reason Company wasn't included
                        var company = _companyRepo.GetById(profile.CompanyId);
                        if (company != null)
                        {
                            company.LogoUrl = logoUrl;
                            _companyRepo.Update(company);
                        }
                    }
                }
                return _profileRepo.Update(profile);
            }
        }

    }
}
