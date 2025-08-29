using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Events.Services;
using OrderProcessingSystem.Events.FileWatcherTasks;
using OrderProcessingSystem.Contracts;
using OrderProcessingSystem.Core.Enums;
using Microsoft.Extensions.Options;

// Create a simple console program to test archiving
Console.WriteLine("Testing Archive Functionality");
Console.WriteLine("==============================");

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        // Configure file naming options
        services.Configure<FileNamingOptions>(options =>
        {
            options.BlobStorage = new BlobStorageNaming
            {
                OrderTransaction = "OrderTransaction.json",
                OrderCancellation = "OrderCancellation.json"
            };
        });
        
        // Register services
        services.AddSingleton<IBlobStorageMonitorService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<BlobStorageMonitorService>>();
            var folderPath = "/Users/sanjeevkumarpaul/Desktop/Code Practice/OrderProcessingSystem/BlobStorageSimulation";
            return new BlobStorageMonitorService(logger, folderPath);
        });
        
        services.AddScoped<OrderFileService>();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
    });

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

var orderFileService = services.GetRequiredService<OrderFileService>();
var monitorService = services.GetRequiredService<IBlobStorageMonitorService>();

Console.WriteLine("Creating test files that will be processed and archived...");

try 
{
    // Create an Order Transaction
    Console.WriteLine("Creating Order Transaction...");
    await orderFileService.CreateOrderTransactionFileAsync("Archive Test Customer", "Archive Test Supplier", 10);
    
    // Wait a moment for processing
    await Task.Delay(3000);
    
    // Create an Order Cancellation
    Console.WriteLine("Creating Order Cancellation...");  
    await orderFileService.CreateOrderCancellationFileAsync("Archive Test Customer", "Archive Test Supplier", 10);
    
    // Wait a moment for processing
    await Task.Delay(3000);
    
    Console.WriteLine("Files created and should be processed. Check the BlobStorageSimulation folder for _Archive directory!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("Test completed. Press any key to exit...");
Console.ReadKey();
