using System.Security.Claims;
using DPCV_API.Configuration;
using DPCV_API.Models;

namespace DPCV_API.BAL.Services.Auth
{
    public interface IAuthService
    {
        Task<ServiceResult> RegisterUserAsync(RegisterDTO registerDto, ClaimsPrincipal user);
        Task<ServiceResult> LoginUserAsync(AuthDTO authDto);
        Task<ServiceResult> RefreshTokenAsync(RefreshTokenDTO refreshTokenDto);
        Task<ServiceResult> LogoutAsync(int userId);
    }
}
