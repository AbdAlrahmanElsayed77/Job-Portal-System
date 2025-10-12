using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    public class BaseTable
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? UpdatedBy { get; set; }

        public int CurrentState { get; set; } 

        public DateTime? CreatedDate { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
