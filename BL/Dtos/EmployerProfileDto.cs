using BL.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos
{
    public class EmployerProfileDto:BaseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }         // FK -> ApplicationUser
        public Guid CompanyId { get; set; }      // FK -> Company
        public string? JobTitle { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CompanyName { get; set; }

    }
}
