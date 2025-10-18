using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.Contracts;
using DAL.DbContext;
using Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BL.Services
{
    public class ApplicationService :BaseService<Application,ApplicationDto>, IApplicationRepository
    {
        private readonly ITableRepository<Application> repo;
        private readonly PortalContext _context;
        private readonly IMapper _mapper;

        public ApplicationService(ITableRepository<Application> repo,PortalContext context, IMapper mapper):base(repo,mapper)
        {
            this.repo = repo;
            _context = context;
            _mapper = mapper;
        }

        public async Task<(bool Success, string Message, Guid? ApplicationId)> ApplyForJobAsync(
            Guid jobPostId,
            Guid jobSeekerId,
            Guid applicantUserId,
            Guid cvFileId,
            string? coverLetter = null)
        {
            try
            {
                var job = await _context.JobPosts.FindAsync(jobPostId);
                if (job == null || !job.IsActive)
                    return (false, "The position is not available for application", null);

                var hasApplied = await _context.Applications
                    .AnyAsync(a => a.JobPostId == jobPostId && a.JobSeekerId == jobSeekerId);

                if (hasApplied)
                    return (false, "You have applied for this job before.", null);

                var cvExists = await _context.CVFiles
                    .AnyAsync(cv => cv.Id == cvFileId && cv.JobSeekerId == jobSeekerId);

                if (!cvExists)
                    return (false, "The specified CV not correct.", null);

                var application = new Domains.Application
                {
                    Id = Guid.NewGuid(),
                    JobPostId = jobPostId,
                    JobSeekerId = jobSeekerId,
                    ApplicantUserId = applicantUserId,
                    CVFileId = cvFileId,
                    CoverLetter = coverLetter,
                    CurrentState = 0, 
                    CreatedDate = DateTime.Now,
                    CreatedBy = applicantUserId
                };

                _context.Applications.Add(application);
                await _context.SaveChangesAsync();

                return (true, "The job application has been successfully submitted.", application.Id);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while submitting.: {ex.Message}", null);
            }
        }


        public async Task<bool> HasAppliedAsync(Guid jobPostId, Guid jobSeekerId)
        {
            return await _context.Applications
                .AnyAsync(a => a.JobPostId == jobPostId && a.JobSeekerId == jobSeekerId);
        }


        public async Task<Guid?> GetApplicationIdAsync(Guid jobPostId, Guid jobSeekerId)
        {
            var app = await _context.Applications
                .Where(a => a.JobPostId == jobPostId && a.JobSeekerId == jobSeekerId)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            return app == Guid.Empty ? null : app;
        }


        public async Task<(List<ApplicationDto> Applications, int TotalCount)> GetJobSeekerApplicationsAsync(
            Guid jobSeekerId,
            int? statusFilter = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.Applications
                .Include(a => a.JobPost)
                    .ThenInclude(j => j.Company)
                .Include(a => a.CVFile)
                .Where(a => a.JobSeekerId == jobSeekerId);

            if (statusFilter.HasValue)
                query = query.Where(a => a.CurrentState == statusFilter.Value);

            if (fromDate.HasValue)
                query = query.Where(a => a.CreatedDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.CreatedDate <= toDate.Value);

            var totalCount = await query.CountAsync();

            var applications = await query
                .OrderByDescending(a => a.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var appDtos = _mapper.Map<List<ApplicationDto>>(applications);

            return (appDtos, totalCount);
        }


        public async Task<ApplicationDto?> GetApplicationDetailsAsync(Guid applicationId, Guid jobSeekerId)
        {
            var application = await _context.Applications
                .Include(a => a.JobPost)
                    .ThenInclude(j => j.Company)
                .Include(a => a.CVFile)
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobSeekerId == jobSeekerId);

            return application == null ? null : _mapper.Map<ApplicationDto>(application);
        }


        public async Task<(bool Success, string Message)> WithdrawApplicationAsync(Guid applicationId, Guid jobSeekerId)
        {
            try
            {
                var application = await _context.Applications
                    .FirstOrDefaultAsync(a => a.Id == applicationId && a.JobSeekerId == jobSeekerId);

                if (application == null)
                    return (false, "Submission not available");

                if (application.CurrentState != 0)
                    return (false, "Submission cannot be withdrawn after review.");

                _context.Applications.Remove(application);
                await _context.SaveChangesAsync();

                return (true, "Submission successfully withdrawn");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred.: {ex.Message}");
            }
        }

        // Get all applications for a specific job post
        public IEnumerable<ApplicationDto> GetApplicationsByJob(Guid jobPostId)
        {
            // Get all applications for a given JobPost, including ApplicantUser for name
            var apps = repo.GetAll(a=>a.ApplicantUser)
                .Where(a => a.JobPostId == jobPostId)
                .ToList();

            // Map to DTOs
            var result = _mapper.Map<IEnumerable<ApplicationDto>>(apps);

            // Fill display-only field (ApplicantName)
            foreach (var dto in result)
            {
                var entity = apps.First(a => a.Id == dto.Id);
                dto.ApplicantName = entity.ApplicantUser?.UserName; // or .UserName if that’s your field
            }

            return result;
        }


        //  Update only the application status
        public void UpdateStatus(Guid id, Status newStatus)
        {
            var app = repo.GetById(id);
            if (app != null)
            {
                app.Status = newStatus;
                app.UpdatedDate = DateTime.UtcNow;
                repo.Update(app);
            }
        }
    }
}