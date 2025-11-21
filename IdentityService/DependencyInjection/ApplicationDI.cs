using IdentityService.Application.DTOs.Tokens;
using IdentityService.Application.Interfaces;
using IdentityService.Application.Services;

namespace IdentityService.DependencyInjection
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddIdentityApplication(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtSettings>(config.GetSection("Jwt"));
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
