using OrderProcessingSystem.Events.Models;
using OrderProcessingSystem.Utilities.Helpers;
using OrderProcessingSystem.Core.Constants;

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
        var solutionRoot = FileHelper.FindSolutionRoot(solutionFileName: FileNames.Solution.OrderProcessingSystem);
        var blobPath = Path.Combine(solutionRoot, FileNames.BlobStorage.DirectoryName);
        
        if (!Directory.Exists(blobPath))
        {
            Directory.CreateDirectory(blobPath);
            _logger.LogInformation($"Created {FileNames.BlobStorage.DirectoryName} directory at: {blobPath}");
        }
        
        return blobPath;
    }

    /// <summary>
    /// Creates a JSON file in the BlobStorageSimulation folder
    /// </summary>
    /// <typeparam name="T">Type of object to serialize</typeparam>
    /// <param name="data">Data to serialize and write</param>
    /// <param name="fileName">Name of the JSON file to create</param>
    private async Task CreateJsonFileAsync<T>(T data, string fileName)
    {
        var jsonContent = JsonHelper.SerializeToJson(data);
        var blobStoragePath = GetBlobStorageSimulationPath();
        await FileHelper.WriteJsonFileAsync(blobStoragePath, fileName, jsonContent, _logger);
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
        
        await CreateJsonFileAsync(orderTransaction, FileNames.BlobStorage.OrderTransaction);
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
        
        await CreateJsonFileAsync(orderCancellation, FileNames.BlobStorage.OrderCancellation);
    }
}
