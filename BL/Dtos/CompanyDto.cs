using BL.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos
{
    public class CompanyDto:BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? LogoUrl { get; set; }
        public string? Description { get; set; }
        public byte Status { get; set; }          // 1=Approved, 0=Pending
        public DateTime CreatedAt { get; set; }
    }
}
