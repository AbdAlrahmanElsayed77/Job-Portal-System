using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class Company :BaseTable
    {

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Website { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(400)]
        public string? LogoUrl { get; set; }

        public string? Description { get; set; }

        // 1=Approved, 0=Pending
        public byte Status { get; set; } = 1;

        // Navs
        public List<EmployerProfile> EmployerProfiles { get; set; } = new List<EmployerProfile>();
        public List<JobPost> JobPosts { get; set; } = new List<JobPost>();
    }
}
