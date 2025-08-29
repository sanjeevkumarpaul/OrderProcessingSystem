using OrderProcessingSystem.Events.Models;
using OrderProcessingSystem.Utilities.Helpers;

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
        var solutionRoot = FileHelper.FindSolutionRoot(solutionFileName: "OrderProcessingSystem.sln");
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
        var price = RandomHelper.GenerateRandomPrice();
        
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
        
        var jsonContent = JsonHelper.SerializeToJson(orderTransaction);
        var blobStoragePath = GetBlobStorageSimulationPath();
        await FileHelper.WriteJsonFileAsync(blobStoragePath, "OrderTransaction.json", jsonContent, _logger);
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
        
        var jsonContent = JsonHelper.SerializeToJson(orderCancellation);
        var blobStoragePath = GetBlobStorageSimulationPath();
        await FileHelper.WriteJsonFileAsync(blobStoragePath, "OrderCancellation.json", jsonContent, _logger);
    }
}
