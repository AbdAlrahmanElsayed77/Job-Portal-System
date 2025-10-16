using AutoMapper;
using BL.Contracts;
using BL.Dtos.AccountDtos;
using Domains.UserModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(IMapper mapper,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<string> RegisterAsync(RegisterDto model)
        {
            var user = _mapper.Map<ApplicationUser>(model);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return string.Join("; ", result.Errors.Select(e => e.Description));
            if (!await _userManager.IsInRoleAsync(user, model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }
            return "User registered successfully!";
        }

        public async Task<string> LoginAsync(LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            return result.Succeeded ? "Login successful!" : "Invalid email or password.";
        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<string> AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return "User not found.";

            await _userManager.AddToRoleAsync(user, role);
            return "Role assigned successfully.";
        }
    }
}
