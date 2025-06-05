using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Group1.BackEnd.Application.Common;
using Group1.BackEnd.Application.DTOs;
using Group1.BackEnd.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Group1.BackEnd.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger;
        }

        private string GenerateJwtToken(IdentityUser user, IList<string> roles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("userId", user.Id)
            };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ServiceResult<string>> RegisterAsync(RegisterDto model)
        {
            var allowedDomains = new List<string> { "gmail.com", "yahoo.com", "outlook.com" };

            var emailParts = model.Email.Split('@');
            if (emailParts.Length != 2 || !allowedDomains.Contains(emailParts[1].ToLower()))
            {
                _logger.LogWarning("Registration failed. Invalid email domain: {Email}", model.Email);
                return ServiceResult<string>.FailureResult("Invalid email domain. Please use a valid email provider.");
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return ServiceResult<string>.FailureResult("Email is already registered.");

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.phoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("User registration failed for {Email}. Reason: {Message}", model.Email, errorMessage);
                return ServiceResult<string>.FailureResult($"User registration failed: {errorMessage}");
            }

            // ✅ Generate email verification token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            // ✅ Get Base URL from Configuration
            var backendBaseUrl = _configuration["AppSettings:BackendBaseUrl"];
            var verificationLink = $"{backendBaseUrl}/api/account/verify-email?email={user.Email}&token={encodedToken}";

            // ✅ Send verification email
            var emailSent = await _emailService.SendEmailAsync(user.Email, "Verify Your Email",
                $"Thank you for registering! Please verify your email by clicking the link below:\n\n{verificationLink}\n\nIf you did not register, please ignore this email.");

            if (!emailSent)
            {
                _logger.LogWarning("User registered, but verification email failed to send for {Email}", user.Email);
                return ServiceResult<string>.FailureResult("User registered, but verification email failed to send.");
            }

            return ServiceResult<string>.SuccessResult("User registered successfully. Please check your email and verify your account before logging in.");
        }

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return ServiceResult<AuthResponseDto>.FailureResult("Invalid email or password.");
            }

            // ✅ Prevent login if email is not verified
            if (!user.EmailConfirmed)
            {
                return ServiceResult<AuthResponseDto>.FailureResult("Email not verified. Please check your email.");
            }

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var token = GenerateJwtToken(user, roles);

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email ?? "",
                Roles = roles
            };

            return ServiceResult<AuthResponseDto>.SuccessResult(response);
        }

        public async Task<ServiceResult<bool>> VerifyEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ServiceResult<bool>.FailureResult("Invalid email or user does not exist.");

            if (user.EmailConfirmed)
                return ServiceResult<bool>.FailureResult("Email is already verified.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Email verification failed for {Email}. Reason: {Message}", email, errorMessage);
                return ServiceResult<bool>.FailureResult("Email verification failed.");
            }

            return ServiceResult<bool>.SuccessResult(true, "Email verified successfully. You can now log in.");
        }

        public async Task<ServiceResult<bool>> DeleteUserByEmailAsync(string email)
        {
            _logger.LogInformation("Attempting to delete user with email: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", email);
                return ServiceResult<bool>.FailureResult("User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to delete user {Email}. Errors: {Errors}", email, errorMessage);
                return ServiceResult<bool>.FailureResult($"Failed to delete user: {errorMessage}");
            }

            _logger.LogInformation("User successfully deleted: {Email}", email);
            return ServiceResult<bool>.SuccessResult(true, "User deleted successfully.");
        }
    }
}
