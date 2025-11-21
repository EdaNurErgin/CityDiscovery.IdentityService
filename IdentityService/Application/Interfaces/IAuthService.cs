using IdentityService.Application.DTOs.Auth;

namespace IdentityService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterRequest request);
        Task<AuthResultDto> LoginAsync(LoginRequest request);
        Task<AuthResultDto> RefreshAsync(string refreshToken, string deviceId);
    }

}
