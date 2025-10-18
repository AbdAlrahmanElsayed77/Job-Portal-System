namespace PortalSystemProject.Models.JobSeeker
{

    public class ApplicationsHistoryViewModel
    {
        public List<ApplicationItemViewModel> Applications { get; set; } = new();

        public string? StatusFilter { get; set; } 
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalApplications { get; set; }
    }

    public class ApplicationItemViewModel
    {
        public Guid ApplicationId { get; set; }
        public Guid JobPostId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public DateTime AppliedAt { get; set; }
        public string Status { get; set; } = string.Empty; 
        public string StatusClass { get; set; } = string.Empty; 
        public string? CoverLetter { get; set; }
        public string? CVFileName { get; set; }
    }


    public class ApplicationDetailsViewModel
    {
        public Guid ApplicationId { get; set; }

        public Guid JobPostId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }

        public DateTime AppliedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CoverLetter { get; set; }
        public string? CVFileName { get; set; }
        public string? CVBlobUrl { get; set; }

        public DateTime? ReviewedAt { get; set; }
        public DateTime? FinalDecisionAt { get; set; }
    }
}