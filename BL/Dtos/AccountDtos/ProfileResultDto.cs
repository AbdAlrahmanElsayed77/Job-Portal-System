using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos.AccountDtos
{
    public class ProfileResultDto
    {
        public object ProfileData { get; set; } = null!;
        public string ViewPath { get; set; } = string.Empty;
        public bool IsEditable { get; set; }
    }
}
