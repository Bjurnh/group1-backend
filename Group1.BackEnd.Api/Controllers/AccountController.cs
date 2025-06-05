using Group1.BackEnd.Application.DTOs;
using Group1.BackEnd.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Group1.BackEnd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Register a new user and send an email verification link.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            _logger.LogInformation("Register request received for Email: {Email}, Phone: {PhoneNumber}", model.Email, model.phoneNumber);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for Email: {Email}", model.Email);
                return BadRequest(new
                {
                    status = 400,
                    title = "Invalid input",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var result = await _authService.RegisterAsync(model);

            if (!result.Success)
            {
                _logger.LogError("Registration failed for Email: {Email}. Reason: {Message}", model.Email, result.Message);
                return BadRequest(new
                {
                    status = 400,
                    title = "Registration failed",
                    message = result.Message
                });
            }

            _logger.LogInformation("User registered successfully. Verification email sent to: {Email}", model.Email);
            return Ok(new
            {
                status = 200,
                title = "Registration successful",
                message = result.Message
            });
        }

        /// <summary>
        /// Verify the email using the token sent to the user's email.
        /// </summary>
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
        {
            _logger.LogInformation("Email verification request received for: {Email}", email);

            var result = await _authService.VerifyEmailAsync(email, token);

            if (!result.Success)
            {
                _logger.LogError("Email verification failed for Email: {Email}. Reason: {Message}", email, result.Message);
                return BadRequest(new
                {
                    status = 400,
                    title = "Email verification failed",
                    message = result.Message
                });
            }

            _logger.LogInformation("Email successfully verified: {Email}", email);
            return Ok(new
            {
                status = 200,
                title = "Email verified",
                message = "Your email has been successfully verified. You can now log in."
            });
        }

        /// <summary>
        /// Log in with a verified email.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authService.LoginAsync(model);

            if (!result.Success)
            {
                _logger.LogWarning("Login attempt failed for Email: {Email}. Reason: {Message}", model.Email, result.Message);
                return Unauthorized(new
                {
                    status = 401,
                    title = "Login failed",
                    message = result.Message
                });
            }

            _logger.LogInformation("User logged in successfully: {Email}", model.Email);
            return Ok(new
            {
                status = 200,
                token = result.Data?.Token,
                email = result.Data?.Email,
                roles = result.Data?.Roles
            });
        }
    }
}
