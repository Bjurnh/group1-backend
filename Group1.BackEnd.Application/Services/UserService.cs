using Microsoft.AspNetCore.Identity;

namespace Group1.BackEnd.Application
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDto> GetUserInfoAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto { Email = user.Email, PhoneNumber = user.PhoneNumber, Roles = roles.ToList() };
        }

        public async Task<bool> UpdateUserInfoAsync(UpdateUserInfoDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return false;

            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            return result.Succeeded;
        }
    }
}
