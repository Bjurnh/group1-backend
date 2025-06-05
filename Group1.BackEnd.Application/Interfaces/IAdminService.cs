using Group1.BackEnd.Application.Common;
using Group1.BackEnd.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group1.BackEnd.Application.Interfaces
{
    public interface IAdminService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<ServiceResult<string>> AddNewUserAsync(RegisterDto model);
        Task<UserDto> GetUserInfoAsync(UserDto email);
        Task<bool> UpdateUserInfoAsync(UpdateUserInfoDto model);
        Task<bool> ChangePasswordAsync(ChangePasswordDto model);
        Task<ServiceResult<string>> DeleteUserAsync(string id);
        Task<ServiceResult<string>> ChangeUserRoleAsync(ChangeRoleDto model);
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<ServiceResult<string>> AddNewRoleAsync(string roleName);
        Task<ServiceResult<string>> DeleteRoleAsync(string id);
    }
}
