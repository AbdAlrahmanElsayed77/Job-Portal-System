using Domains.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class Application:BaseTable
    {
        public string? CoverLetter { get; set; }
        public Guid JobPostId { get; set; }
        public JobPost JobPost { get; set; } = null!;

        public Guid JobSeekerId { get; set; }
        public JobSeekerProfile JobSeeker { get; set; } = null!;

        public Guid ApplicantUserId { get; set; }
        public ApplicationUser ApplicantUser { get; set; } = null!;

        public Guid? CVFileId { get; set; }
        public CVFile? CVFile { get; set; }
 
    }
}
