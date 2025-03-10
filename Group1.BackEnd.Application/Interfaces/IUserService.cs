using Group1.BackEnd.Domain.Entities;

namespace Group1.BackEnd.Application
{
    public interface IUserService
    {
        Task<UserDto> GetUserInfoAsync(string email);
        Task<bool> UpdateUserInfoAsync(UpdateUserInfoDto model);
        Task<bool> DeleteUserAsync(string email);
        Task<bool> ChangePasswordAsync(ChangePasswordDto model);
    }
}
