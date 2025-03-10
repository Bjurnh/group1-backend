using Group1.BackEnd.Application.DTOs;
using Group1.BackEnd.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApiWithRoleAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var result = await _authService.RegisterAsync(model);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);
            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }
            return Ok(new
            {
                token = result.Data?.Token,
                email = result.Data?.Email,
                roles = result.Data?.Roles
            });
        }
    }
}

