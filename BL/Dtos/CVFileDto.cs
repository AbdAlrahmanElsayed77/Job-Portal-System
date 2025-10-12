using BL.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos
{
    public class CVFileDto:BaseDto
    {
        public Guid JobSeekerId { get; set; }     // FK -> JobSeekerProfile
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string BlobUrl { get; set; } = string.Empty;
        public int FileSizeBytes { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
