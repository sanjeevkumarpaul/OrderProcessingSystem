using Microsoft.Extensions.DependencyInjection;
using OrderProcessingSystem.Infrastructure.Services;
using MediatR;
using System.Reflection;

namespace OrderProcessingSystem.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ApiDataService>();
        
        // Register MediatR and handlers from the API project
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.Load("OrderProcessingSystem.API"));
        
        return services;
    }
}
