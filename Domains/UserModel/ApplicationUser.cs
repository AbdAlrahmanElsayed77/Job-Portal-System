
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.UserModel
{
    public class ApplicationUser : IdentityUser<Guid>
    {

        public EmployerProfile? EmployerProfile { get; set; }
        public JobSeekerProfile? JobSeekerProfile { get; set; }
        public List<JobPost> CreatedJobPosts { get; set; } = new List<JobPost>();
        public List<Application> Applications { get; set; } = new List<Application>();
    }
}
