namespace IdentityService.Application.DTOs.Tokens
{
    public class JwtSettings { public string Issuer { get; set; } = default!; public string Audience { get; set; } = default!; public string Key { get; set; } = default!; public int AccessTokenMinutes { get; set; } = 60; }

}
