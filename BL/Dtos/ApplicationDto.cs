using BL.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos
{
    public class ApplicationDto:BaseDto
    {
        public Guid JobPostId { get; set; }       // FK -> JobPost
        public Guid JobSeekerId { get; set; }     // FK -> JobSeekerProfile
        public Guid ApplicantUserId { get; set; } // FK -> ApplicationUser
        public string? CoverLetter { get; set; }
        public Guid? CVFileId { get; set; }       // FK -> CVFile (snapshot)
        public byte Status { get; set; }          // 0..6
        public DateTime AppliedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
