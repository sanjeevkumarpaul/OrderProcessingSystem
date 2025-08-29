using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Events.Services;
using OrderProcessingSystem.Events.FileWatcherTasks;
using OrderProcessingSystem.Contracts;
using Microsoft.Extensions.Options;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.Configure<FileNamingOptions>(options =>
        {
            options.BlobStorage = new BlobStorageNaming
            {
                OrderTransaction = "OrderTransaction.json",
                OrderCancellation = "OrderCancellation.json"
            };
        });
        
        services.AddSingleton<IBlobStorageMonitorService, BlobStorageMonitorService>();
        services.AddScoped<OrderFileService>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var orderFileService = scope.ServiceProvider.GetRequiredService<OrderFileService>();

Console.WriteLine("Creating test files...");

// Create an Order Transaction
await orderFileService.CreateOrderTransactionFileAsync("Archive Test Customer", "Archive Test Supplier", 25);

// Create an Order Cancellation  
await orderFileService.CreateOrderCancellationFileAsync("Archive Test Customer", "Archive Test Supplier", 25);

Console.WriteLine("Test files created! Check BlobStorageSimulation folder.");
Console.WriteLine("Press any key to continue...");
Console.ReadKey();
