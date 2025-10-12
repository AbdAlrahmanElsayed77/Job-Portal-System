using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class SavedJob:BaseTable
    {
        public Guid JobSeekerId { get; set; }
        public JobSeekerProfile JobSeeker { get; set; } = null!;

        public Guid JobPostId { get; set; }
        public JobPost JobPost { get; set; } = null!;
    }
}
