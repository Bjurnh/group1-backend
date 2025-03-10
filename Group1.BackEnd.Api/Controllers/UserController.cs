using Group1.BackEnd.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiWithRoleAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RoleUser")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("user-info")]
        public async Task<IActionResult> GetUserInfo([FromBody] string email)
        {
            var user = await _userService.GetUserInfoAsync(email);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [HttpPut("user-info")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoDto model)
        {
            var success = await _userService.UpdateUserInfoAsync(model);
            if (!success)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(new { message = "User Info Updated Successfully." });
        }

        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUserAccount([FromBody] string email)
        {
            var success = await _userService.DeleteUserAsync(email);
            if (!success)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(new { message = "User deleted successfully." });
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordDto model)
        {
            var success = await _userService.ChangePasswordAsync(model);
            if (!success)
            {
                return NotFound(new { message = "User not found or password incorrect." });
            }

            return Ok(new { message = "Password changed successfully." });
        }
    }
}
