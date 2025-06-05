using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Group1.BackEnd.Application.DTOs;
using Group1.BackEnd.Application.Interfaces;
using Group1.BackEnd.Application;

namespace Group1.BackEnd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "RoleAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost("users")]
        public async Task<IActionResult> AddNewUser([FromBody] RegisterDto model)
        {
            var result = await _adminService.AddNewUserAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message, errors = result.Data });
            }

            return Ok(new { message = result.Message, userId = result.Data });
        }


        [HttpPost("admin-info")]
        public async Task<IActionResult> GetUserInfo([FromBody] UserDto email)
        {
            var user = await _adminService.GetUserInfoAsync(email);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [HttpPut("admin-info")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserInfoDto model)
        {
            var success = await _adminService.UpdateUserInfoAsync(model);
            if (!success)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(new { message = "User Info Updated Successfully." });
        }

        [HttpPut("change-admin-password")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordDto model)
        {
            var success = await _adminService.ChangePasswordAsync(model);
            if (!success)
            {
                return NotFound(new { message = "User not found or password incorrect." });
            }

            return Ok(new { message = "Password changed successfully." });
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpPost("change-user-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeRoleDto model)
        {
            var result = await _adminService.ChangeUserRoleAsync(model);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _adminService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpPost("roles")]
        public async Task<IActionResult> AddNewRole([FromBody] RoleDto roleDto)
        {
            var result = await _adminService.AddNewRoleAsync(roleDto.Name);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpDelete("roles/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var result = await _adminService.DeleteRoleAsync(id);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }
    }
}
