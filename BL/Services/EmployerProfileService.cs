using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.Contracts;
using Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
                profile.CreatedBy = userId;
                profile.CreatedDate = DateTime.UtcNow;
                profile.CurrentState = 1;

                if (logoFile != null)
                {
                    var logoUrl = _fileService.UploadFileAsync("company_logos", logoFile).Result;

                    var company = _companyRepo.GetById(dto.CompanyId);
                    if (company != null)
                    {
                        company.LogoUrl = logoUrl;
                        company.UpdatedBy = userId;
                        company.UpdatedDate = DateTime.UtcNow;
                        _companyRepo.Update(company);
                    }
                    else
                    {
                        company = new Company()
                        {
                            LogoUrl = logoUrl,
                            CreatedBy = userId,
                            CreatedDate = DateTime.UtcNow
                        };
                        _companyRepo.Add(company);
                    }
                    profile.CompanyId = company.Id;
                }
                
                return _profileRepo.Add(profile);
            }
            else
            {
                // 🟠 Update existing
                profile = _profileRepo.GetById(dto.Id);
                if (profile == null)
                    throw new Exception("Profile not found");

                // Preserve CompanyId - don't let it be changed
                var originalCompanyId = profile.CompanyId;

                _mapper.Map(dto, profile);
                profile.CompanyId = originalCompanyId; // ✅ Ensure CompanyId stays the same
                profile.UpdatedBy = userId;
                profile.UpdatedDate = DateTime.UtcNow;

                // ✅ Handle logo update separately to avoid tracking conflicts
                if (logoFile != null)
                {
                    var logoUrl = _fileService.UploadFileAsync("company_logos", logoFile).Result;

                    // Get a fresh instance of the company to avoid tracking conflicts
                    var company = _companyRepo.GetById(profile.CompanyId);
                    if (company != null)
                    {
                        company.LogoUrl = logoUrl;
                        company.UpdatedBy = userId;
                        company.UpdatedDate = DateTime.UtcNow;
                        _companyRepo.Update(company);
                    }
                }

                return _profileRepo.Update(profile);
            }
        }
    }
}