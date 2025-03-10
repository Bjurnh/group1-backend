using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Group1.BackEnd.Application.Common;
using Group1.BackEnd.Application.DTOs;
using Group1.BackEnd.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Group1.BackEnd.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

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


        public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<ServiceResult<string>> RegisterAsync(RegisterDto model)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.phoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return ServiceResult<string>.FailureResult("User registration failed.");
            }

            return ServiceResult<string>.SuccessResult("User registered successfully.");
        }

        public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return ServiceResult<AuthResponseDto>.FailureResult("Invalid email or password.");
            }

            var roles = (await _userManager.GetRolesAsync(user)).ToList(); 

            var token = GenerateJwtToken(user, roles); 

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                Roles = roles
            };

            return ServiceResult<AuthResponseDto>.SuccessResult(response);
        }


    }
}
