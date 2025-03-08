using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using WebApiWithRoleAuthentication.Models;
using Group1.BackEnd.Interfaces;

namespace Group1.BackEnd.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IValidator<Register> _validator;

        public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IValidator<Register> validator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _validator = validator;
        }

        public async Task<(bool Success, string Message, IEnumerable<string>? Errors)> RegisterUserAsync(Register model)
        {
            var validationResult = await _validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                return (false, "Validation failed.", errors);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return (false, "Email already exists.", null);
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.phoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return (false, "User registration failed.", result.Errors.Select(e => e.Description));
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return (false, "User role creation failed.", roleResult.Errors.Select(e => e.Description));
                }
            }

            await _userManager.AddToRoleAsync(user, "User");

            return (true, "User registered successfully", null);
        }
    }
}
