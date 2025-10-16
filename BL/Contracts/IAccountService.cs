using BL.Dtos.AccountDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Contracts
{
    public interface IAccountService
    {
        Task<string> RegisterAsync(RegisterDto model);
        Task<string> LoginAsync(LoginDto model);
        Task LogoutAsync();
        Task<string> AssignRoleAsync(string userId, string role);
    }
}
