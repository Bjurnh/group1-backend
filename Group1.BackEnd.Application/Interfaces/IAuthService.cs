using Group1.BackEnd.Application.Common;
using Group1.BackEnd.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group1.BackEnd.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<string>> RegisterAsync(RegisterDto model);
        Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto model);
    }
}
