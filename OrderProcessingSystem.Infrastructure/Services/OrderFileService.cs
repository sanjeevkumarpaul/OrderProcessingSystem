using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Events.Models;
using OrderProcessingSystem.Utilities.Helpers;
using OrderProcessingSystem.Core.Configuration;
using OrderProcessingSystem.Core.Enums;
using Microsoft.Extensions.Options;
using OrderProcessingSystem.Contracts.Interfaces;

namespace OrderProcessingSystem.Infrastructure.Services;

/// <summary>
/// Service for creating order-related JSON files in the BlobStorageSimulation folder
/// </summary>
public class OrderFileService : IOrderFileService
{
    private readonly ILogger<OrderFileService> _logger;
    private readonly FileNamingOptions _fileNamingOptions;
    private readonly IBlobStorageMonitorService? _blobStorageMonitorService;

    public OrderFileService(
        ILogger<OrderFileService> logger, 
        IOptions<FileNamingOptions> fileNamingOptions,
        IBlobStorageMonitorService? blobStorageMonitorService = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileNamingOptions = fileNamingOptions?.Value ?? throw new ArgumentNullException(nameof(fileNamingOptions));
        _blobStorageMonitorService = blobStorageMonitorService; // Optional - can be null
    }

    /// <summary>
    /// Gets the BlobStorageSimulation directory path, creating it if it doesn't exist
    /// </summary>
    /// <returns>Full path to the BlobStorageSimulation directory</returns>
    private string GetBlobStoragePath()
    {
        var solutionRoot = FileHelper.FindSolutionRoot(solutionFileName: _fileNamingOptions.Solution.OrderProcessingSystem);
        var blobPath = Path.Combine(solutionRoot, _fileNamingOptions.BlobStorage.DirectoryName);
        
        FileHelper.EnsureDirectoryExists(blobPath, _logger);
        
        return blobPath;
    }

    /// <summary>
    /// Generates a unique filename with timestamp postfix
    /// </summary>
    /// <param name="baseFileName">Base filename (e.g., "OrderTransaction.json")</param>
    /// <returns>Unique filename with timestamp postfix</returns>
    private string GenerateUniqueFileName(string baseFileName)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(baseFileName);
        var extension = Path.GetExtension(baseFileName);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"); // Include milliseconds for uniqueness
        
        return $"{fileNameWithoutExtension}_{timestamp}{extension}";
    }

    /// <summary>
    /// Creates a JSON file in the BlobStorageSimulation folder
    /// </summary>
    /// <typeparam name="T">Type of object to serialize</typeparam>
    /// <param name="data">Data to serialize to JSON</param>
    /// <param name="baseFileName">Base name of the file to create</param>
    /// <param name="queue">Which queue to use for processing</param>
    /// <returns>Task representing the asynchronous operation</returns>
    private async Task CreateJsonFileAsync<T>(T data, string baseFileName, FileProcessingQueue queue)
    {
        var blobStoragePath = GetBlobStoragePath();
        var uniqueFileName = GenerateUniqueFileName(baseFileName);
        var jsonContent = JsonHelper.SerializeToJson(data);
        
        await FileHelper.WriteJsonFileAsync(blobStoragePath, uniqueFileName, jsonContent, _logger);
        
        // Queue the file for processing by the background service (use unique filename) - only if service is available
        if (_blobStorageMonitorService != null)
        {
            await _blobStorageMonitorService.QueueFileProcessingTask(uniqueFileName, queue);
        }
    }

    /// <summary>
    /// Creates an OrderTransaction JSON file with sample data
    /// </summary>
    /// <param name="customerName">Name of the customer</param>
    /// <param name="supplierName">Name of the supplier</param>
    /// <param name="quantity">Quantity of items ordered</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task CreateOrderTransactionFileAsync(string customerName, string supplierName, int quantity)
    {
        var price = RandomHelper.GenerateRandomPrice();
        
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

        await CreateJsonFileAsync(orderTransaction, _fileNamingOptions.BlobStorage.OrderTransaction, FileProcessingQueue.OrderTransaction);
    }

    /// <summary>
    /// Creates an OrderCancellation JSON file with sample data
    /// </summary>
    /// <param name="customerName">Name of the customer</param>
    /// <param name="supplierName">Name of the supplier</param>
    /// <param name="quantity">Quantity of items to cancel</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task CreateOrderCancellationFileAsync(string customerName, string supplierName, int quantity)
    {
        var orderCancellation = new OrderCancellationSchema
        {
            Customer = customerName,
            Supplier = supplierName,
            Quantity = quantity
        };

        await CreateJsonFileAsync(orderCancellation, _fileNamingOptions.BlobStorage.OrderCancellation, FileProcessingQueue.OrderCancellation);
    }
}
