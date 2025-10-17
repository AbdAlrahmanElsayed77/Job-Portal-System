using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IEmailService
    {
        Task<string> SendEmailAsync(string to, string subject, string body);
    }
}
