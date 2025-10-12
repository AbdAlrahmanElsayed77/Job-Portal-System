using Domains.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class EmployerProfile:BaseTable
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        [MaxLength(120)]
        public string? JobTitle { get; set; }

    }
}
