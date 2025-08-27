using Microsoft.Extensions.DependencyInjection;
using OrderProcessingSystem.Core.Entities;
using OrderProcessingSystem.Core.Interfaces;
using OrderProcessingSystem.Infrastructure.Repositories;
using OrderProcessingSystem.Infrastructure.Services;

namespace OrderProcessingSystem.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // register in-memory repository and domain services for now
        services.AddSingleton<IRepository<Order>, OrderProcessingSystem.Infrastructure.Repositories.OrderRepository>();
        services.AddScoped<IOrderService, OrderService>();
        // register API-facing data service which returns Contracts DTOs
        services.AddScoped<ApiDataService>();
        return services;
    }
}
