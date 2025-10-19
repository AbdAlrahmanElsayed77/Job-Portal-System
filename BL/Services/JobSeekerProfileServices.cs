using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.Contracts;
using Domains;
using Microsoft.AspNetCore.Http;

namespace BL.Services
{
    public class JobSeekerProfileServices : BaseService<JobSeekerProfile, JobSeekerProfileDto>, IJobSeekerProfileService
    {
        private readonly ITableRepository<JobSeekerProfile> _profileRepo;
        private readonly ITableRepository<CVFile> _cvRepo;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public JobSeekerProfileServices(
            ITableRepository<JobSeekerProfile> profileRepo,
            ITableRepository<CVFile> cvRepo,
            IFileService fileService,
            IMapper mapper) : base(profileRepo, mapper)
        {
            _profileRepo = profileRepo;
            _cvRepo = cvRepo;
            _fileService = fileService;
            _mapper = mapper;
        }

        public JobSeekerProfileDto GetByUserId(Guid userId)
        {
            var entity = _profileRepo.GetAll()
                .FirstOrDefault(p => p.UserId == userId);
            return _mapper.Map<JobSeekerProfileDto>(entity);
        }

        public JobSeekerProfileDto GetOrCreate(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return new JobSeekerProfileDto();

            var entity = _profileRepo.GetById(id.Value);
            if (entity == null)
                return new JobSeekerProfileDto();

            return _mapper.Map<JobSeekerProfileDto>(entity);
        }

        public void SaveProfile(JobSeekerProfileDto dto, Guid userId, IFormFile? photo, IFormFile? cv)
        {
            JobSeekerProfile profile;

            if (dto.Id == Guid.Empty)
            {
                // 🟢 Create new profile
                profile = _mapper.Map<JobSeekerProfile>(dto);
                profile.UserId = userId;
                profile.CreatedBy = userId;
                profile.CreatedDate = DateTime.UtcNow;
                profile.CurrentState = 1;

                if (photo != null)
                {
                    profile.PhotoUrl = _fileService.UploadFileAsync("uploads/profile_photos", photo).Result;
                }

                // ✅ First, save profile alone so EF generates its ID
                _profileRepo.Add(profile);

                // ✅ Now attach CV if provided
                if (cv != null)
                {
                    var cvEntity = new CVFile
                    {
                        Id = Guid.NewGuid(),
                        JobSeekerId = profile.Id, // ✅ critical line - now profile.Id is generated
                        BlobUrl = _fileService.UploadFileAsync("uploads/cvs", cv).Result,
                        FileName = cv.FileName,
                        ContentType = cv.ContentType,
                        FileSizeBytes = (int)cv.Length,
                        IsPrimary = true,
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                        CurrentState = 1
                    };

                    _cvRepo.Add(cvEntity);
                }
            }
            else
            {
                // 🟠 Update existing profile
                profile = _profileRepo.GetById(dto.Id);
                if (profile == null)
                    throw new Exception("Profile not found");

                // Preserve original values
                var originalUserId = profile.UserId;
                var originalCreatedBy = profile.CreatedBy;
                var originalCreatedDate = profile.CreatedDate;

                _mapper.Map(dto, profile);

                // ✅ Restore preserved values
                profile.UserId = originalUserId;
                profile.CreatedBy = originalCreatedBy;
                profile.CreatedDate = originalCreatedDate;
                profile.UpdatedBy = userId;
                profile.UpdatedDate = DateTime.UtcNow;

                if (photo != null)
                {
                    profile.PhotoUrl = _fileService.UploadFileAsync("uploads/profile_photos", photo).Result;
                }

                _profileRepo.Update(profile);

                // ✅ Add new CV if provided
                if (cv != null)
                {
                    var cvEntity = new CVFile
                    {
                        Id = Guid.NewGuid(),
                        JobSeekerId = profile.Id, // ✅ ensure FK is valid
                        BlobUrl = _fileService.UploadFileAsync("uploads/cvs", cv).Result,
                        FileName = cv.FileName,
                        ContentType = cv.ContentType,
                        FileSizeBytes = (int)cv.Length,
                        IsPrimary = false, // Existing profile, so not primary
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow,
                        CurrentState = 1
                    };
                    _cvRepo.Add(cvEntity);
                }
            }
        }
    }
}