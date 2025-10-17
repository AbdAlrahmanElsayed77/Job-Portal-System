using System.ComponentModel.DataAnnotations;

namespace PortalSystemProject.Models.JobSeeker
{

    public class ApplyJobViewModel
    {
        public Guid JobPostId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select your CV")]
        [Display(Name = "CV")]
        public Guid CVFileId { get; set; }

        [Display(Name = "Cover Letter")]
        [MaxLength(2000, ErrorMessage = "The cover letter cannot exceed 2,000 characters.")]
        public string? CoverLetter { get; set; }

        public List<CVOption> AvailableCVs { get; set; } = new();
    }

    public class ApplyJobResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? ApplicationId { get; set; }
    }
}