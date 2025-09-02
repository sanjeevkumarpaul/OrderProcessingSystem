using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using OrderProcessingSystem.Data.Repositories;
using OrderProcessingSystem.Data.Interfaces;
using OrderProcessingSystem.Contracts.Interfaces;


namespace OrderProcessingSystem.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderProcessingData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));
        
        // SQL Provider will be registered in Infrastructure layer
        // services.AddSingleton<ISqlProvider, SqlFileProvider>(); // Moved to Infrastructure
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<ITransExceptionRepository, TransExceptionRepository>();
        services.AddMediatR(typeof(ServiceCollectionExtensions).Assembly);
        return services;
    }
}
