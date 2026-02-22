using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace IdentityService.Infrastructure.Search;
public static class ElasticsearchExtensions
{
    public static IServiceCollection AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        // docker-compose üzerinden erişim için url: http://elasticsearch:9200 
        // Localhost için: http://localhost:9200
        var elasticUrl = configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
        var settings = new ElasticsearchClientSettings(new Uri(elasticUrl))
            .DefaultIndex("users"); // Varsayılan index adımız

        var client = new ElasticsearchClient(settings);

        // Dependency Injection container'a ekliyoruz
        services.AddSingleton(client);

        return services;
    }
}