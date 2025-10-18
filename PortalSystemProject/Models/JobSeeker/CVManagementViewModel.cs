using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PortalSystemProject.Models.JobSeeker
{

    public class CVManagementViewModel
    {
        public List<CVItemViewModel> CVFiles { get; set; } = new();
        public UploadCVViewModel UploadModel { get; set; } = new();
    }

    public class CVItemViewModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string BlobUrl { get; set; } = string.Empty;
        public int FileSizeBytes { get; set; }
        public string FileSizeDisplay => FormatFileSize(FileSizeBytes);
        public bool IsPrimary { get; set; }
        public DateTime UploadedAt { get; set; }
        public int UsedInApplications { get; set; } 

        private string FormatFileSize(int bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024} KB";
            return $"{bytes / (1024 * 1024)} MB";
        }
    }


    public class UploadCVViewModel
    {
        [Required(ErrorMessage = "Please select your resume")]
        [Display(Name = "CV")]
        public IFormFile CVFile { get; set; } = null!;

        [Display(Name = "Set as primary CV")]
        public bool SetAsPrimary { get; set; }
    }


    public class CVUploadResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? CVFileId { get; set; }
    }
}