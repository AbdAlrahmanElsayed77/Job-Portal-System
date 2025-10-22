using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.DbContext;
using Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BL.Services
{
    public class SavedJobService : ISavedJobRepository
    {
        private readonly PortalContext _context;
        private readonly IMapper _mapper;

        public SavedJobService(PortalContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<(bool Success, string Message)> SaveJobAsync(Guid jobPostId, Guid jobSeekerId)
        {
            try
            {
                var jobExists = await _context.JobPosts.AnyAsync(j => j.Id == jobPostId);
                if (!jobExists)
                    return (false, "The job is not available");

                var alreadySaved = await _context.SavedJobs
                    .AnyAsync(s => s.JobPostId == jobPostId && s.JobSeekerId == jobSeekerId);

                if (alreadySaved)
                    return (false, "The job is already saved");

                var savedJob = new SavedJob
                {
                    Id = Guid.NewGuid(),
                    JobPostId = jobPostId,
                    JobSeekerId = jobSeekerId,
                    CreatedDate = DateTime.Now,
                    CreatedBy = jobSeekerId,
                    CurrentState = 1
                };

                _context.SavedJobs.Add(savedJob);
                await _context.SaveChangesAsync();

                return (true, "The job was saved successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }


        public async Task<(bool Success, string Message)> UnsaveJobAsync(Guid jobPostId, Guid jobSeekerId)
        {
            try
            {
                var savedJob = await _context.SavedJobs
                    .FirstOrDefaultAsync(s => s.JobPostId == jobPostId && s.JobSeekerId == jobSeekerId);

                if (savedJob == null)
                    return (false, "Job not saved");

                _context.SavedJobs.Remove(savedJob);
                await _context.SaveChangesAsync();

                return (true, "The job was successfully unsaved.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }


        public async Task<bool> IsJobSavedAsync(Guid jobPostId, Guid jobSeekerId)
        {
            return await _context.SavedJobs
                .AnyAsync(s => s.JobPostId == jobPostId && s.JobSeekerId == jobSeekerId);
        }


        public async Task<List<SavedJobDto>> GetSavedJobsAsync(Guid jobSeekerId)
        {
            var savedJobs = await _context.SavedJobs
                .Include(s => s.JobPost)
                    .ThenInclude(j => j.Company)
                .Include(s => s.JobPost.JobCategory)
                .Include(s => s.JobPost.JobType)
                .Where(s => s.JobSeekerId == jobSeekerId)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();

            return _mapper.Map<List<SavedJobDto>>(savedJobs);
        }


        public async Task<List<Guid>> GetSavedJobIdsAsync(Guid jobSeekerId)
        {
            return await _context.SavedJobs
                .Where(s => s.JobSeekerId == jobSeekerId)
                .Select(s => s.JobPostId)
                .ToListAsync();
        }
        public async Task<List<SavedJobDto>> GetSavedJobsWithDetailsAsync(Guid jobSeekerId)
        {
            var savedJobs = await _context.SavedJobs
                .Where(s => s.JobSeekerId == jobSeekerId)
                .Include(s => s.JobPost)
                    .ThenInclude(j => j.Company)
                .Include(s => s.JobPost)
                    .ThenInclude(j => j.JobCategory)
                .Include(s => s.JobPost)
                    .ThenInclude(j => j.JobType)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();

            return _mapper.Map<List<SavedJobDto>>(savedJobs);
        }
    }
}