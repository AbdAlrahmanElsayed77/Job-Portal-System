namespace PortalSystemProject.Models.JobSeeker
{

    public class SavedJobsViewModel
    {
        public List<SavedJobItemViewModel> SavedJobs { get; set; } = new();
        public int TotalSaved { get; set; }
    }

    public class SavedJobItemViewModel
    {
        public Guid JobPostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public string? Location { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? JobType { get; set; }
        public string? SalaryRange { get; set; }
        public DateTime SavedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        public bool HasApplied { get; set; }
        public bool IsActive { get; set; } 
    }
}