using BL.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos
{
    public class JobPostDto:BaseDto
    {
        public Guid CompanyId { get; set; }       // FK -> Company
        public Guid CreatedByUserId { get; set; } // FK -> ApplicationUser
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Requirements { get; set; }
        public Guid JobCategoryId { get; set; }    // FK -> JobCategory
        public Guid? JobTypeId { get; set; }      // FK -> JobType
        public string? Country { get; set; }
        public string? City { get; set; }
        public byte? MinExperienceYears { get; set; }
        public byte? MaxExperienceYears { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? Currency { get; set; }     // e.g., "EGP", "USD"
        public bool IsActive { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
