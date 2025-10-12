using BL.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos
{
    public class SavedJobDto:BaseDto
    {
        public Guid JobSeekerId { get; set; }     // part of composite key
        public Guid JobPostId { get; set; }       // part of composite key
        public DateTime SavedAt { get; set; }
    }
}
