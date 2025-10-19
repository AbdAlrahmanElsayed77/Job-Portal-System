using BL.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos
{
    public class JobSeekerProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public byte? YearsOfExperience { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PhotoUrl { get; set; }
        public string? UserEmail { get; set; }

        // Navigation properties
        public List<ProfileCVFileDto> CVFiles { get; set; } = new();
        public List<ProfileApplicationDto> Applications { get; set; } = new();
        public List<ProfileSavedJobDto> SavedJobs { get; set; } = new();
    }
    // ApplicationDto - Enhanced with display properties
    public class ProfileApplicationDto : BaseDto
    {
        public Guid JobPostId { get; set; }
        public Guid JobSeekerId { get; set; }
        public string? CoverLetter { get; set; }
        public int Status { get; set; } // 0=Pending, 1=Accepted, 2=Rejected
        public DateTime AppliedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // For display purposes
        public string? JobPostTitle { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyLogoUrl { get; set; }
    }
    // CVFileDto
    public class ProfileCVFileDto : BaseDto
    {
        public Guid JobSeekerId { get; set; }
        public string BlobUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public int FileSizeBytes { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    // SavedJobDto
    public class ProfileSavedJobDto : BaseDto
    {
        public Guid JobSeekerId { get; set; }
        public Guid JobPostId { get; set; }
        public DateTime SavedAt { get; set; }

        // For display
        public string? JobTitle { get; set; }
        public string? CompanyName { get; set; }
    }
}
