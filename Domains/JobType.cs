using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class JobType:BaseTable
    {
        [Required, MaxLength(60)]
        public string Name { get; set; } = string.Empty;

        public List<JobPost> JobPosts { get; set; } = new List<JobPost>();
    }
}
