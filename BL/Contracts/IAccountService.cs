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
        Task<string> RegisterAsync(RegisterDto model, string origin);
        Task<string> LoginAsync(LoginDto model);
        Task<string> ConfirmEmailAsync(Guid userId, string token);
        Task<string> ForgotPasswordAsync(string email, string origin);
        Task<string> ResetPasswordAsync(string email, string token, string newPassword);
        Task LogoutAsync();
        Task<ProfileViewResult> GetProfile(Guid viewerId, Guid targetUserId);
    }
}
