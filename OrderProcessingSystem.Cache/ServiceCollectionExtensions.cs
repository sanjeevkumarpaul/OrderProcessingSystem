using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace OrderProcessingSystem.Cache;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderProcessingCache(this IServiceCollection services, IConfiguration config)
    {
        // For now, use in-memory caching to avoid Redis dependency issues
        // TODO: Add Redis support later when needed
        services.AddScoped<IGridColumnCacheService, InMemoryGridColumnCacheService>();
        
        return services;
    }
}
