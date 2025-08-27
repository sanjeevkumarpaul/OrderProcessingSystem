using Microsoft.Extensions.DependencyInjection;
using OrderProcessingSystem.Infrastructure.Services;

namespace OrderProcessingSystem.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ApiDataService>();
        return services;
    }
}
