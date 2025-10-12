using Domains.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class JobSeekerProfile:BaseTable
    {

        [MaxLength(140)]
        public string? Headline { get; set; }

        public string? Summary { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        public byte? YearsOfExperience { get; set; }

        // Navs

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public List<CVFile> CVFiles { get; set; } = new List<CVFile>();
        public List<Application> Applications { get; set; } = new List<Application>();
        public List<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();
    }
}
