using Group1.BackEnd.Application.Common;
using Group1.BackEnd.Application.DTOs;
using Group1.BackEnd.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Group1.BackEnd.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            return users.Select(user => new UserDto
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = _userManager.GetRolesAsync(user).Result.ToList()
            }).ToList();
        }

        public async Task<ServiceResult<string>> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new ServiceResult<string>(false, "User not found.", null);
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                return new ServiceResult<string>(false, "Admin cannot be deleted.", null);
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return new ServiceResult<string>(true, "User deleted successfully.", id);
            }

            return new ServiceResult<string>(false, "Failed to delete user.", null);
        }


        public async Task<ServiceResult<string>> ChangeUserRoleAsync(ChangeRoleDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserEmail);
            if (user == null)
            {
                return new ServiceResult<string>(false, "User not found.", null);
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                return new ServiceResult<string>(false, "Admin role cannot be changed.", null);
            }

            if (!await _roleManager.RoleExistsAsync(model.NewRole))
            {
                return new ServiceResult<string>(false, $"Role {model.NewRole} does not exist.", null);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                return new ServiceResult<string>(false, "Failed to remove user's current role.", null);
            }

            var addResult = await _userManager.AddToRoleAsync(user, model.NewRole);
            if (addResult.Succeeded)
            {
                return new ServiceResult<string>(true, $"User role changed to {model.NewRole} successfully.", model.NewRole);
            }

            return new ServiceResult<string>(false, "Failed to add user to the new role.", null);
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.Select(role => new RoleDto { Id = role.Id, Name = role.Name }).ToListAsync();
            return roles;
        }

        public async Task<ServiceResult<string>> AddNewRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return new ServiceResult<string>(false, "Role already exists.", null);
            }

            var result = await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
            if (result.Succeeded)
            {
                return new ServiceResult<string>(true, "Role added successfully.", roleName);
            }

            return new ServiceResult<string>(false, "Failed to add role.", null);
        }


        public async Task<ServiceResult<string>> DeleteRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return new ServiceResult<string>(false, "Role not found.", null);
            }

            if (role.Name == "Admin")
            {
                return new ServiceResult<string>(false, "Admin role cannot be deleted.", null);
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return new ServiceResult<string>(true, "Role deleted successfully.", id);
            }

            return new ServiceResult<string>(false, "Failed to delete role.", null);
        }

    }
}
