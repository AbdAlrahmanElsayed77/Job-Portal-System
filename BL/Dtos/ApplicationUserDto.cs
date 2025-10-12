using BL.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos
{
    public class ApplicationUserDto :BaseDto
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string? UserName { get; set; }         // optional (from Identity)
        public string? Email { get; set; }            // optional (from Identity)
        public DateTime CreatedAt { get; set; }
    }
}
