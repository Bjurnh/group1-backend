using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Group1.BackEnd.Application.DTOs;
using Group1.BackEnd.Application.Interfaces;

namespace WebApiWithRoleAuthentication.Controllers
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
        public async Task<IActionResult> AddNewRole([FromBody] string roleName)
        {
            var result = await _adminService.AddNewRoleAsync(roleName);
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
