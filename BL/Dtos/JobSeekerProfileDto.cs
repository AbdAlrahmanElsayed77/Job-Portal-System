using BL.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos
{
    public class JobSeekerProfileDto:BaseDto
    {
        public Guid UserId { get; set; }          // FK -> ApplicationUser
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public byte? YearsOfExperience { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
