using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Dtos.AccountDtos
{
    public class ProfileViewResult
    {
        public string ViewPath { get; set; } = string.Empty;
        public object ProfileData { get; set; } = null!;
        public bool IsEditable { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public bool ProfileExists { get; set; }
    }
}
