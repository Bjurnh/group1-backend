using Group1.BackEnd.Interfaces;
using Group1.BackEnd.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiWithRoleAuthentication.Models;

namespace WebApiWithRoleAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var result = await _userService.RegisterUserAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message, errors = result.Errors });
            }

            return Ok(new { message = result.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim("userId", user.Id)
                };

                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                    SecurityAlgorithms.HmacSha256));

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return Unauthorized();
        }

        private bool isValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
