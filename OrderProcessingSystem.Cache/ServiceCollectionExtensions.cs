using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace OrderProcessingSystem.Cache;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderProcessingCache(this IServiceCollection services, IConfiguration config)
    {
        // Expects configuration key "Redis:Configuration" e.g. "localhost:6379"
    var redisConfig = config["Redis:Configuration"];
    services.AddStackExchangeRedisCache(opts => opts.Configuration = redisConfig);
        services.AddSingleton<ICacheService, RedisCacheService>();
        return services;
    }
}
