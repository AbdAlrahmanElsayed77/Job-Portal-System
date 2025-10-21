using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos
{
    public class JobStatusToggleDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
