using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using BL.Dtos.AccountDtos;
using Domains.UserModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IJobSeekerProfileService _jobSeekerService;
        private readonly IEmployerProfileService _employerService;
        public AccountService(IMapper mapper,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailService, IJobSeekerProfileService jobSeekerService, IEmployerProfileService employerService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _jobSeekerService = jobSeekerService;
            _employerService = employerService;
        }

        public async Task<string> RegisterAsync(RegisterDto model, string origin)
        {
            var user = _mapper.Map<ApplicationUser>(model);
            user.EmailConfirmed = false;
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return string.Join("; ", result.Errors.Select(e => e.Description));
            if (!await _userManager.IsInRoleAsync(user, model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            // Generate email confirmation link
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var confirmUrl = $"{origin}/Account/ConfirmEmail?userId={user.Id}&token={encodedToken}";

            var body = $"<h3>Welcome to Portal System</h3>" +
                       $"<p>Please confirm your email by clicking the link below:</p>" +
                       $"<a href='{confirmUrl}'>Confirm Email</a>";

            await _emailService.SendEmailAsync(user.Email, "Confirm your account", body);

            return "Registration successful! Please check your email to confirm your account.";
        }
        public async Task<string> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return "Invalid email or password.";

            if (!user.EmailConfirmed)
                return "Please confirm your email before logging in.";

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
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
        public async Task<string> ConfirmEmailAsync(Guid userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return "User not found.";

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded ? "Email confirmed successfully!" : "Invalid or expired token.";
        }
        public async Task<string> ForgotPasswordAsync(string email, string origin)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return "User not found.";

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var resetUrl = $"{origin}/Account/ResetPassword?email={email}&token={encodedToken}";

            var body = $"<h3>Password Reset</h3>" +
                       $"<p>Click the link below to reset your password:</p>" +
                       $"<a href='{resetUrl}'>Reset Password</a>";

            await _emailService.SendEmailAsync(email, "Reset your password", body);

            return "Password reset link sent to your email.";
        }
        public async Task<string> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return "User not found.";

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded ? "Password reset successfully!" : "Failed to reset password.";
        }
        public async Task<ProfileResultDto> GetProfile(Guid viewerId, Guid targetUserId)
        {
            var viewer = await _userManager.FindByIdAsync(viewerId.ToString());
            var targetUser = await _userManager.FindByIdAsync(targetUserId.ToString());

            if (targetUser == null)
                throw new Exception("User not found");

            bool isEditable = viewerId == targetUserId;

            // 🔹 Get target user's role
            var roles = await _userManager.GetRolesAsync(targetUser);
            var role = roles.FirstOrDefault();

            if (string.IsNullOrEmpty(role))
                throw new Exception("User role not assigned");

            // 🔸 Job Seeker
            if (role == "JobSeeker")
            {
                var profile = _jobSeekerService.GetByUserId(targetUserId);
                if (profile == null)
                {
                    if (isEditable)
                    {
                        // redirect user to create profile if viewing their own
                        return new ProfileResultDto
                        {
                            ProfileData = new JobSeekerProfileDto { UserId = targetUserId },
                            ViewPath = "~/Views/JobSeekerProfile/Edit.cshtml",
                            IsEditable = true
                        };
                    }

                    throw new Exception("Job Seeker profile not found");
                }

                return new ProfileResultDto
                {
                    ProfileData = profile,
                    ViewPath = "~/Views/JobSeekerProfile/Index.cshtml",
                    IsEditable = isEditable
                };
            }

            // 🔸 Employer
            if (role == "Employer")
            {
                var profile = _employerService.GetByUserId(targetUserId);
                if (profile == null)
                {
                    if (isEditable)
                    {
                        // redirect user to create profile if viewing their own
                        return new ProfileResultDto
                        {
                            ProfileData = new EmployerProfileDto { UserId = targetUserId },
                            ViewPath = "~/Views/EmployerProfile/Edit.cshtml",
                            IsEditable = true
                        };
                    }

                    throw new Exception("Employer profile not found");
                }

                return new ProfileResultDto
                {
                    ProfileData = profile,
                    ViewPath = "~/Views/EmployerProfile/Index.cshtml",
                    IsEditable = isEditable
                };
            }

            throw new Exception("Unknown user role");
        }
    }

}
