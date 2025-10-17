namespace PortalSystemProject.Models.JobSeeker
{

    public class JobDetailsViewModel
    {
        public Guid JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Requirements { get; set; }

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? CompanyDescription { get; set; }

        public string Category { get; set; } = string.Empty;
        public string? JobType { get; set; }
        public string? Location { get; set; } 
        public string? ExperienceRequired { get; set; } 
        public string? SalaryRange { get; set; }  
        public DateTime PublishedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public bool IsSaved { get; set; }
        public bool HasApplied { get; set; }
        public bool CanApply { get; set; } = true; 
        public Guid? CurrentApplicationId { get; set; }

        public List<CVOption> AvailableCVs { get; set; } = new();

        public int TotalApplications { get; set; }
    }

    public class CVOption
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}