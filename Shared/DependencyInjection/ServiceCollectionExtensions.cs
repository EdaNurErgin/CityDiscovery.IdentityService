using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services;

namespace Shared.DependencyInjection
{
    /// <summary>
    /// Shared kütüphanesi için DI extension metodları
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// IdentityService ile HTTP iletişimi için gerekli servisleri kaydeder
        /// </summary>
        public static IServiceCollection AddIdentityHttpClient(this IServiceCollection services, string identityServiceBaseUrl)
        {
            services.AddHttpClient<IdentityHttpClient>(client =>
            {
                client.BaseAddress = new Uri(identityServiceBaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }

        /// <summary>
        /// IdentityService ile HTTP iletişimi için gerekli servisleri kaydeder (Configuration'dan URL alır)
        /// </summary>
        public static IServiceCollection AddIdentityHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            var identityServiceUrl = configuration["Services:IdentityService:BaseUrl"] ?? "http://identity-service";
            return services.AddIdentityHttpClient(identityServiceUrl);
        }
    }
}
