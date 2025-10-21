using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using DAL.DbContext;
using Domains;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BL.Services
{
    public class CVFileService : ICVFileRepository
    {
        private readonly PortalContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private const int MaxFileSizeBytes = 5 * 1024 * 1024;

        private readonly string[] _allowedExtensions = { ".pdf", ".docx", ".doc" };

        public CVFileService(PortalContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<CVFileDto>> GetCVsByJobSeekerIdAsync(Guid jobSeekerId)
        {
            var cvFiles = await _context.CVFiles
                .Where(cv => cv.JobSeekerId == jobSeekerId)
                .OrderByDescending(cv => cv.IsPrimary)
                .ThenByDescending(cv => cv.CreatedDate)
                .ToListAsync();

            return _mapper.Map<List<CVFileDto>>(cvFiles);
        }

        public async Task<(bool Success, string Message, Guid? CVFileId)> UploadCVAsync(
            Guid jobSeekerId,
            IFormFile file,
            bool setAsPrimary = false)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (false, "Please select a valid file.", null);

                if (file.Length > MaxFileSizeBytes)
                    return (false, "File size must not exceed 5 MB.", null);

                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                    return (false, "File type not allowed. Allowed types: PDF, DOCX, DOC", null);

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "cvs");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var cvFile = new CVFile
                {
                    Id = Guid.NewGuid(),
                    JobSeekerId = jobSeekerId,
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    BlobUrl = $"/uploads/cvs/{uniqueFileName}", // Fixed path
                    FileSizeBytes = (int)file.Length,
                    IsPrimary = setAsPrimary,
                    CreatedDate = DateTime.Now,
                    CreatedBy = jobSeekerId,
                    CurrentState = 1
                };

                if (setAsPrimary)
                {
                    var existingPrimary = await _context.CVFiles
                        .Where(cv => cv.JobSeekerId == jobSeekerId && cv.IsPrimary)
                        .ToListAsync();

                    foreach (var cv in existingPrimary)
                    {
                        cv.IsPrimary = false;
                    }
                }

                _context.CVFiles.Add(cvFile);
                await _context.SaveChangesAsync();

                return (true, "CV uploaded successfully", cvFile.Id);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while uploading the file: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> DeleteCVAsync(Guid cvFileId, Guid jobSeekerId)
        {
            try
            {
                var cvFile = await _context.CVFiles
                    .FirstOrDefaultAsync(cv => cv.Id == cvFileId && cv.JobSeekerId == jobSeekerId);

                if (cvFile == null)
                    return (false, "CV not found");

                var usageCount = await _context.Applications
                    .CountAsync(a => a.CVFileId == cvFileId);

                if (usageCount > 0)
                    return (false, "The resume cannot be deleted because it has been used in previous submissions.");

                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, cvFile.BlobUrl.TrimStart('/'));
                if (File.Exists(filePath))
                    File.Delete(filePath);

                _context.CVFiles.Remove(cvFile);
                await _context.SaveChangesAsync();

                return (true, "CV deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while deleting: {ex.Message}");
            }
        }

        public async Task<bool> SetPrimaryAsync(Guid cvFileId, Guid jobSeekerId)
        {
            try
            {
                var allCVs = await _context.CVFiles
                    .Where(cv => cv.JobSeekerId == jobSeekerId)
                    .ToListAsync();

                foreach (var cv in allCVs)
                {
                    cv.IsPrimary = cv.Id == cvFileId;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //  Keep only one version with nullable handling
        public async Task<List<CVFileDto>> GetCVsByIdsAsync(List<Guid> ids)
        {
            var validIds = ids.Where(id => id != Guid.Empty).ToList();

            if (!validIds.Any())
                return new List<CVFileDto>();

            var cvs = await _context.CVFiles
                .Where(c => validIds.Contains(c.Id))
                .ToListAsync();

            return _mapper.Map<List<CVFileDto>>(cvs);
        }

        //  Keep only one version with nullable handling
        public async Task<CVFileDto?> GetCVByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            var cv = await _context.CVFiles
                .FirstOrDefaultAsync(c => c.Id == id);

            return cv != null ? _mapper.Map<CVFileDto>(cv) : null;
        }

        public async Task<CVFileDto?> GetPrimaryCVAsync(Guid jobSeekerId)
        {
            var cv = await _context.CVFiles
                .FirstOrDefaultAsync(cv => cv.JobSeekerId == jobSeekerId && cv.IsPrimary);

            return cv == null ? null : _mapper.Map<CVFileDto>(cv);
        }

        public async Task<int> GetCVUsageCountAsync(Guid cvFileId)
        {
            return await _context.Applications
                .CountAsync(a => a.CVFileId == cvFileId);
        }
    }
}