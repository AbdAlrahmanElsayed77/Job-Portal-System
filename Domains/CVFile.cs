using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class CVFile:BaseTable
    {
        public Guid JobSeekerId { get; set; }
        public JobSeekerProfile JobSeeker { get; set; } = null!;

        [Required, MaxLength(200)]
        public string FileName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string ContentType { get; set; } = string.Empty; // validate in service (PDF/DOCX)

        [Required, MaxLength(500)]
        public string BlobUrl { get; set; } = string.Empty;

        public int FileSizeBytes { get; set; }

        public bool IsPrimary { get; set; }

        // Nav back: Applications may snapshot this CV
        public List<Application> Applications { get; set; } = new List<Application>();
    }
}
