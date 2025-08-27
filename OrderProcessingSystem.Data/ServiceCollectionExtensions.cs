using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using OrderProcessingSystem.Data.Repositories;
using OrderProcessingSystem.Data.Interfaces;


namespace OrderProcessingSystem.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderProcessingData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));
    // Register SQL file provider from Core (reads embedded resources)
    services.AddSingleton<OrderProcessingSystem.Core.Sql.ISqlProvider, OrderProcessingSystem.Core.Sql.SqlFileProvider>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddMediatR(typeof(ServiceCollectionExtensions).Assembly);
        return services;
    }
}
