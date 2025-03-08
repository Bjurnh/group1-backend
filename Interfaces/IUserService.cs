using WebApiWithRoleAuthentication.Models;

namespace Group1.BackEnd.Interfaces
{
    public interface IUserService
    {
        Task<(bool Success, string Message, IEnumerable<string>? Errors)> RegisterUserAsync(Register model);
    }
}
