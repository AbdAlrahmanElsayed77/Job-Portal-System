using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.DbContext;
using Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BL.Services
{
    public class JobSeekerProfileService : IJobSeekerProfileRepository
    {
        private readonly PortalContext _context;
        private readonly IMapper _mapper;

        public JobSeekerProfileService(PortalContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<JobSeekerProfileDto?> GetByUserIdAsync(Guid userId)
        {
            var profile = await _context.JobSeekerProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return profile == null ? null : _mapper.Map<JobSeekerProfileDto>(profile);
        }


        public async Task<bool> HasProfileAsync(Guid userId)
        {
            return await _context.JobSeekerProfiles
                .AnyAsync(p => p.UserId == userId);
        }

        public async Task<Guid> CreateProfileAsync(JobSeekerProfileDto profileDto)
        {
            var profile = _mapper.Map<JobSeekerProfile>(profileDto);
            profile.Id = Guid.NewGuid();
            profile.CreatedDate = DateTime.Now;
            profile.CreatedBy = profileDto.UserId;
            profile.CurrentState = 1;

            _context.JobSeekerProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return profile.Id;
        }


        public async Task<bool> UpdateProfileAsync(JobSeekerProfileDto profileDto)
        {
            try
            {
                var profile = await _context.JobSeekerProfiles
                    .FirstOrDefaultAsync(p => p.Id == profileDto.Id);

                if (profile == null)
                    return false;

                profile.Headline = profileDto.Headline;
                profile.Summary = profileDto.Summary;
                profile.Country = profileDto.Country;
                profile.City = profileDto.City;
                profile.YearsOfExperience = profileDto.YearsOfExperience;
                profile.UpdatedDate = DateTime.Now;
                profile.UpdatedBy = profileDto.UserId;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}