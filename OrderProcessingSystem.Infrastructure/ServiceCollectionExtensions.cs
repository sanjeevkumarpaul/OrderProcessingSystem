using Microsoft.Extensions.DependencyInjection;
using OrderProcessingSystem.Infrastructure.Services;
using OrderProcessingSystem.Infrastructure.Providers;
using OrderProcessingSystem.Contracts.Interfaces;
using MediatR;
using System.Reflection;

namespace OrderProcessingSystem.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Infrastructure services for API projects (direct database access)
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IApiDataService, ApiDataService>();
        services.AddScoped<IOrderFileService, OrderFileService>();
        services.AddScoped<IUserLogService, UserLogService>();
        
        // Register SQL Provider implementation in Infrastructure layer (Clean Architecture)
        services.AddSingleton<ISqlProvider, SqlFileProvider>();
        
        // Register MediatR and handlers from the Infrastructure project
        services.AddMediatR(Assembly.GetExecutingAssembly());
        
        return services;
    }
    
    /// <summary>
    /// Registers Infrastructure services for client applications (HTTP-based API access)
    /// Use this for UI projects and other clients that should call APIs via HTTP
    /// </summary>
    public static IServiceCollection AddInfrastructureHttpServices(this IServiceCollection services)
    {
        services.AddScoped<IApiDataService, HttpApiDataService>();
        services.AddScoped<IOrderFileService, OrderFileService>();
        services.AddScoped<IUserLogService, HttpUserLogService>();
        
        // Register SQL Provider implementation (may be needed for some services)
        services.AddSingleton<ISqlProvider, SqlFileProvider>();
        
        // Register MediatR and handlers from the Infrastructure project
        services.AddMediatR(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
