using OrderProcessingSystem.Events.Models;
using System.Text.Json;

namespace OrderProcessingServer.Services;

public class OrderFileService
{
    private readonly ILogger<OrderFileService> _logger;

    public OrderFileService(ILogger<OrderFileService> logger)
    {
        _logger = logger;
    }

    private string GetBlobStorageSimulationPath()
    {
        // Get solution root directory (go up from OrderProcessingServer/bin/Debug/net9.0 to solution root)
        var currentDir = Directory.GetCurrentDirectory();
        var solutionRoot = currentDir;
        
        // Keep going up until we find the solution root (contains OrderProcessingSystem.sln)
        while (solutionRoot != null && !File.Exists(Path.Combine(solutionRoot, "OrderProcessingSystem.sln")))
        {
            solutionRoot = Directory.GetParent(solutionRoot)?.FullName;
        }
        
        if (solutionRoot == null)
        {
            throw new DirectoryNotFoundException("Could not find solution root directory");
        }
        
        var blobPath = Path.Combine(solutionRoot, "BlobStorageSimulation");
        
        if (!Directory.Exists(blobPath))
        {
            Directory.CreateDirectory(blobPath);
            _logger.LogInformation($"Created BlobStorageSimulation directory at: {blobPath}");
        }
        
        return blobPath;
    }

    public async Task CreateOrderTransactionFileAsync(string customerName, string supplierName, int quantity)
    {
        // Generate a random price between 200 and 1000 (validation range)
        var random = new Random();
        var price = Math.Round((decimal)(random.NextDouble() * 800 + 200), 2); // 200-1000 range
        
        // Create OrderTransaction model
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = supplierName,
                Quantity = quantity,
                Price = price
            },
            Customer = new CustomerInfoSchema
            {
                Name = customerName,
                Quantity = quantity,
                Price = price
            }
        };
        
        // Serialize to JSON
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonContent = JsonSerializer.Serialize(orderTransaction, jsonOptions);
        
        // Write to file in BlobStorageSimulation folder
        var blobStoragePath = GetBlobStorageSimulationPath();
        var filePath = Path.Combine(blobStoragePath, "OrderTransaction.json");
        
        _logger.LogInformation($"Creating OrderTransaction.json at: {filePath}");
        await File.WriteAllTextAsync(filePath, jsonContent);
        _logger.LogInformation("OrderTransaction.json created successfully");
    }
    
    public async Task CreateOrderCancellationFileAsync(string customerName, string supplierName, int quantity)
    {
        // Create OrderCancellation model
        var orderCancellation = new OrderCancellationSchema
        {
            Customer = customerName,
            Supplier = supplierName,
            Quantity = quantity
        };
        
        // Serialize to JSON
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonContent = JsonSerializer.Serialize(orderCancellation, jsonOptions);
        
        // Write to file in BlobStorageSimulation folder
        var blobStoragePath = GetBlobStorageSimulationPath();
        var filePath = Path.Combine(blobStoragePath, "OrderCancellation.json");
        
        _logger.LogInformation($"Creating OrderCancellation.json at: {filePath}");
        await File.WriteAllTextAsync(filePath, jsonContent);
        _logger.LogInformation("OrderCancellation.json created successfully");
    }
}
