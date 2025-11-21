namespace IdentityService.Application.DTOs.Auth
{
    public class AuthResultDto { public string AccessToken { get; set; } = default!; public string RefreshToken { get; set; } = default!; public Guid UserId { get; set; } }

}
