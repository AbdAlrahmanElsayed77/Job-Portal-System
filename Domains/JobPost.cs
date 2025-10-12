using Domains.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class JobPost:BaseTable
    {
       

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string? Requirements { get; set; }


 
        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public byte? MinExperienceYears { get; set; }
        public byte? MaxExperienceYears { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxSalary { get; set; }

        [MaxLength(3)]
        public string? Currency { get; set; }

        public bool IsActive { get; set; } = true;

        // Navs

        public Guid JobCategoryId { get; set; }
        public JobCategory JobCategory { get; set; } = null!;
        public Guid? JobTypeId { get; set; }
        public JobType? JobType { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public Guid CreatedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; } = null!;
        public List<Application> Applications { get; set; } = new List<Application>();
        public List<SavedJob> SavedBy { get; set; } = new List<SavedJob>();
    }
}
