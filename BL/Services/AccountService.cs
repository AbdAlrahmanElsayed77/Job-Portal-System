using AutoMapper;
using BL.Contracts;
using BL.Dtos;
using BL.Dtos.AccountDtos;
using DAL.Contracts;
using Domains;
using Domains.UserModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ITableRepository<JobSeekerProfile> _jobSeekerRepo;
        private readonly ITableRepository<EmployerProfile> _employerRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            ITableRepository<JobSeekerProfile> jobSeekerRepo,
            ITableRepository<EmployerProfile> employerRepo,
            IMapper mapper,
            ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _jobSeekerRepo = jobSeekerRepo;
            _employerRepo = employerRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<string> RegisterAsync(RegisterDto model, string origin)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                    return "User with this email already exists.";

                var user = _mapper.Map<ApplicationUser>(model);
                user.Id = Guid.NewGuid();

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return string.Join(", ", result.Errors.Select(e => e.Description));

                await _userManager.AddToRoleAsync(user, model.Role);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var confirmationLink = $"{origin}/Account/ConfirmEmail?userId={user.Id}&token={encodedToken}";

                await _emailService.SendEmailAsync(
                    model.Email,
                    "Confirm Your Email",
                    $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>."
                );

                _logger.LogInformation("User {Email} registered successfully", model.Email);
                return "Registration successful! Please check your email to confirm your account.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Email}", model.Email);
                return "An error occurred during registration. Please try again.";
            }
        }

        public async Task<string> LoginAsync(LoginDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return "Invalid email or password.";

                if (!user.EmailConfirmed)
                    return "Please confirm your email before logging in.";

                var result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false
                );

                if (!result.Succeeded)
                    return "Invalid email or password.";

                _logger.LogInformation("User {Email} logged in successfully", model.Email);
                return "Login successful!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", model.Email);
                return "An error occurred during login. Please try again.";
            }
        }

        public async Task<string> ConfirmEmailAsync(Guid userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return "User not found.";

                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Email confirmed for user {UserId}", userId);
                    return "Email confirmed successfully! You can now log in.";
                }

                return "Error confirming email. The link may have expired.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming email for user {UserId}", userId);
                return "An error occurred while confirming your email.";
            }
        }

        public async Task<string> ForgotPasswordAsync(string email, string origin)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return "If the email exists, a reset link has been sent.";

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var resetLink = $"{origin}/Account/ResetPassword?email={email}&token={encodedToken}";

                await _emailService.SendEmailAsync(
                    email,
                    "Reset Your Password",
                    $"Reset your password by clicking <a href='{resetLink}'>here</a>."
                );

                _logger.LogInformation("Password reset link sent to {Email}", email);
                return "If the email exists, a reset link has been sent.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password for {Email}", email);
                return "If the email exists, a reset link has been sent.";
            }
        }

        public async Task<string> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return "Invalid request.";

                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password reset successful for {Email}", email);
                    return "Password reset successful! You can now log in with your new password.";
                }

                return "Error resetting password. The link may have expired.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for {Email}", email);
                return "An error occurred while resetting your password.";
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
        }

        public async Task<ProfileViewResult> GetProfile(Guid viewerId, Guid targetUserId)
        {
            var result = new ProfileViewResult
            {
                IsEditable = viewerId == targetUserId
            };

            try
            {
                var targetUser = await _userManager.FindByIdAsync(targetUserId.ToString());
                if (targetUser == null)
                {
                    _logger.LogWarning("User {UserId} not found", targetUserId);
                    result.ViewPath = "~/Views/Shared/Error.cshtml";
                    result.ProfileExists = false;
                    return result;
                }

                var roles = await _userManager.GetRolesAsync(targetUser);
                var role = roles.FirstOrDefault() ?? "Unknown";
                result.UserRole = role;

                if (role == "JobSeeker")
                {
                    var profile = _jobSeekerRepo
                        .GetAll(p => p.CVFiles)
                        .FirstOrDefault(p => p.UserId == targetUserId);

                    if (profile == null)
                    {
                        result.ProfileExists = false;
                        result.ViewPath = result.IsEditable
                            ? "~/Views/JobSeekerProfile/Edit.cshtml"
                            : "~/Views/Shared/ProfileNotFound.cshtml";
                        result.ProfileData = new JobSeekerProfileDto { UserId = targetUserId };

                        _logger.LogInformation("JobSeeker profile not found for user {UserId}", targetUserId);
                    }
                    else
                    {
                        result.ProfileExists = true;
                        result.ViewPath = "~/Views/JobSeekerProfile/Index.cshtml";
                        result.ProfileData = _mapper.Map<JobSeekerProfileDto>(profile);

                        _logger.LogInformation("JobSeeker profile loaded for user {UserId}", targetUserId);
                    }
                }
                else if (role == "Employer")
                {
                    var profile = _employerRepo
                        .GetAll(p => p.Company)
                        .FirstOrDefault(p => p.UserId == targetUserId);

                    if (profile == null)
                    {
                        result.ProfileExists = false;
                        result.ViewPath = result.IsEditable
                            ? "~/Views/EmployerProfile/Edit.cshtml"
                            : "~/Views/Shared/ProfileNotFound.cshtml";
                        result.ProfileData = new EmployerProfileDto { UserId = targetUserId };

                        _logger.LogInformation("Employer profile not found for user {UserId}", targetUserId);
                    }
                    else
                    {
                        result.ProfileExists = true;
                        result.ViewPath = "~/Views/EmployerProfile/Index.cshtml";
                        result.ProfileData = _mapper.Map<EmployerProfileDto>(profile);

                        _logger.LogInformation("Employer profile loaded for user {UserId}", targetUserId);
                    }
                }
                else
                {
                    _logger.LogWarning("Unknown role {Role} for user {UserId}", role, targetUserId);
                    result.ViewPath = "~/Views/Shared/Error.cshtml";
                    result.ProfileExists = false;
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profile for user {UserId}", targetUserId);
                result.ViewPath = "~/Views/Shared/Error.cshtml";
                result.ProfileExists = false;
                return result;
            }
        }
    }
}