using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderProcessingSystem.Contracts.Interfaces;
using OrderProcessingSystem.Events.Configurations;
using OrderProcessingSystem.Events.Models;
using OrderProcessingSystem.Utilities.Helpers;
using OrderProcessingSystem.Core.Enums;
using System.Collections.Concurrent;
using System.Text.Json;

namespace OrderProcessingSystem.Events.FileWatcherTasks;

public class BlobStorageMonitorService : BackgroundService, IBlobStorageMonitorService
{
    private readonly ILogger<BlobStorageMonitorService> _logger;
    private readonly BlobStorageSimulationOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _fullFolderPath;
    private readonly ConcurrentQueue<string> _orderTransactionQueue;
    private readonly ConcurrentQueue<string> _orderCancellationQueue;
    private readonly SemaphoreSlim _transactionQueueSemaphore;
    private readonly SemaphoreSlim _cancellationQueueSemaphore;

    public BlobStorageMonitorService(
        ILogger<BlobStorageMonitorService> logger,
        IOptions<BlobStorageSimulationOptions> options,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _orderTransactionQueue = new ConcurrentQueue<string>();
        _orderCancellationQueue = new ConcurrentQueue<string>();
        _transactionQueueSemaphore = new SemaphoreSlim(0);
        _cancellationQueueSemaphore = new SemaphoreSlim(0);
        
        // Convert relative path to absolute path
        _fullFolderPath = Path.GetFullPath(_options.FolderPath);
        
        _logger.LogInformation("BlobStorageMonitorService initialized with separate queues. Monitoring folder: {FolderPath}", _fullFolderPath);
    }

    /// <summary>
    /// Queues a task to process a specific file in the monitored folder
    /// </summary>
    /// <param name="fileName">Name of the file to process</param>
    /// <param name="queue">Which queue to use for processing</param>
    /// <returns>Task representing the queuing operation</returns>
    public async Task QueueFileProcessingTask(string fileName, FileProcessingQueue queue)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            _logger.LogWarning("Cannot queue file processing task: fileName is null or empty");
            return;
        }

        switch (queue)
        {
            case FileProcessingQueue.OrderTransaction:
                _logger.LogInformation("Queuing ORDER TRANSACTION processing task for: {FileName}", fileName);
                _orderTransactionQueue.Enqueue(fileName);
                _transactionQueueSemaphore.Release();
                break;
            case FileProcessingQueue.OrderCancellation:
                _logger.LogInformation("Queuing ORDER CANCELLATION processing task for: {FileName}", fileName);
                _orderCancellationQueue.Enqueue(fileName);
                _cancellationQueueSemaphore.Release();
                break;
            default:
                _logger.LogWarning("Unknown queue type: {Queue}", queue);
                break;
        }
        
        await Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BlobStorageMonitorService started - Separate queue processing mode");

        // Ensure the directory exists
        FileHelper.EnsureDirectoryExists(_fullFolderPath, _logger);

        // Create tasks for processing both queues concurrently
        var transactionTask = ProcessTransactionQueue(stoppingToken);
        var cancellationTask = ProcessCancellationQueue(stoppingToken);

        // Wait for both tasks to complete
        await Task.WhenAll(transactionTask, cancellationTask);

        _logger.LogInformation("BlobStorageMonitorService stopped");
    }

    private async Task ProcessTransactionQueue(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Transaction queue processor started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Wait for a transaction file to be queued or cancellation
                await _transactionQueueSemaphore.WaitAsync(stoppingToken);
                
                if (_orderTransactionQueue.TryDequeue(out string? fileName))
                {
                    await ProcessQueuedFile(fileName, FileProcessingQueue.OrderTransaction);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during transaction queue processing");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Brief delay before continuing
            }
        }
        
        _logger.LogInformation("Transaction queue processor stopped");
    }

    private async Task ProcessCancellationQueue(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cancellation queue processor started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Wait for a cancellation file to be queued or cancellation
                await _cancellationQueueSemaphore.WaitAsync(stoppingToken);
                
                if (_orderCancellationQueue.TryDequeue(out string? fileName))
                {
                    await ProcessQueuedFile(fileName, FileProcessingQueue.OrderCancellation);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cancellation queue processing");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Brief delay before continuing
            }
        }
        
        _logger.LogInformation("Cancellation queue processor stopped");
    }

    private async Task ProcessQueuedFile(string fileName, FileProcessingQueue queue)
    {
        try
        {
            var filePath = Path.Combine(_fullFolderPath, fileName);
            
            // Check if file exists
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Queued file not found: {FilePath}", filePath);
                return;
            }

            _logger.LogInformation("Processing queued file from {Queue} queue: {FilePath}", queue, filePath);
            
            // Process file based on queue type
            switch (queue)
            {
                case FileProcessingQueue.OrderTransaction:
                    await ProcessOrderTransactionFile(filePath);
                    break;
                case FileProcessingQueue.OrderCancellation:
                    await ProcessOrderCancellationFile(filePath);
                    break;
                default:
                    _logger.LogWarning("Unknown queue type for file: {FileName}", fileName);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing queued file from {Queue} queue: {FileName}", queue, fileName);
        }
    }

    private async Task ProcessJsonFile(string filePath)
    {
        try
        {
            var fileName = Path.GetFileName(filePath).ToLower();
            
            // Wait a bit to ensure file write is complete
            await Task.Delay(100);

            // Check if file is still being written to
            if (IsFileLocked(filePath))
            {
                _logger.LogWarning("File is locked, skipping: {FilePath}", filePath);
                return;
            }

            if (fileName == "ordertransaction.json")
            {
                await ProcessOrderTransactionFile(filePath);
            }
            else if (fileName == "ordercancellation.json")
            {
                await ProcessOrderCancellationFile(filePath);
            }
            else
            {
                _logger.LogInformation("Unknown JSON file type: {FileName}", fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing JSON file: {FilePath}", filePath);
        }
    }

    private async Task ProcessOrderTransactionFile(string filePath)
    {
        try
        {
            // Wait a bit to ensure file write is complete
            await Task.Delay(100);

            // Check if file is still being written to
            if (IsFileLocked(filePath))
            {
                _logger.LogDebug("File {FilePath} is locked, waiting...", filePath);
                await Task.Delay(1000);
                if (IsFileLocked(filePath))
                {
                    _logger.LogWarning("File {FilePath} still locked after waiting", filePath);
                    return;
                }
            }

            var fileContent = await File.ReadAllTextAsync(filePath);
            
            if (string.IsNullOrWhiteSpace(fileContent))
            {
                _logger.LogWarning("File {FilePath} is empty", filePath);
                return;
            }

            _logger.LogInformation("Processing order transaction file: {FilePath}", filePath);
            _logger.LogDebug("File content: {Content}", fileContent);

            // Try to parse JSON and validate schema
            try
            {
                var orderTransaction = JsonSerializer.Deserialize<OrderTransactionSchema>(fileContent);
                
                if (orderTransaction == null)
                {
                    _logger.LogError("Failed to deserialize order transaction from {FilePath}", filePath);
                    return;
                }

                // Validate the schema
                if (!ValidateOrderTransaction(orderTransaction))
                {
                    _logger.LogError("Order transaction validation failed for {FilePath}", filePath);
                    return;
                }

                _logger.LogInformation("Successfully validated and parsed order transaction from {FilePath}", filePath);
                _logger.LogInformation("Order Transaction - Supplier: {SupplierName}, Customer: {CustomerName}, Quantity: {Quantity}, Price: {Price}", 
                    orderTransaction.Supplier.Name, orderTransaction.Customer.Name, 
                    orderTransaction.Supplier.Quantity, orderTransaction.Supplier.Price);
                
                await ProcessOrderTransaction(orderTransaction, filePath);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON format in order transaction file {FilePath}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order transaction file {FilePath}", filePath);
        }
    }

    private async Task ProcessOrderCancellationFile(string filePath)
    {
        try
        {
            // Wait a bit to ensure file write is complete
            await Task.Delay(100);

            // Check if file is still being written to
            if (IsFileLocked(filePath))
            {
                _logger.LogDebug("File {FilePath} is locked, waiting...", filePath);
                await Task.Delay(1000);
                if (IsFileLocked(filePath))
                {
                    _logger.LogWarning("File {FilePath} still locked after waiting", filePath);
                    return;
                }
            }

            var fileContent = await File.ReadAllTextAsync(filePath);
            
            if (string.IsNullOrWhiteSpace(fileContent))
            {
                _logger.LogWarning("File {FilePath} is empty", filePath);
                return;
            }

            _logger.LogInformation("Processing order cancellation file: {FilePath}", filePath);
            _logger.LogDebug("File content: {Content}", fileContent);

            // Try to parse JSON and validate schema
            try
            {
                var orderCancellation = JsonSerializer.Deserialize<OrderCancellationSchema>(fileContent);
                
                if (orderCancellation == null)
                {
                    _logger.LogError("Failed to deserialize order cancellation from {FilePath}", filePath);
                    return;
                }

                // Validate the schema
                if (!ValidateOrderCancellation(orderCancellation))
                {
                    _logger.LogError("Order cancellation validation failed for {FilePath}", filePath);
                    return;
                }

                _logger.LogInformation("Successfully validated and parsed order cancellation from {FilePath}", filePath);
                _logger.LogInformation("Order Cancellation - Customer: {Customer}, Supplier: {Supplier}, Quantity: {Quantity}", 
                    orderCancellation.Customer, orderCancellation.Supplier, orderCancellation.Quantity);
                
                await ProcessOrderCancellation(orderCancellation, filePath);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON format in order cancellation file {FilePath}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order cancellation file {FilePath}", filePath);
        }
    }

    private bool ValidateOrderTransaction(OrderTransactionSchema orderTransaction)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(orderTransaction.Supplier.Name))
            {
                _logger.LogError("Supplier name is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(orderTransaction.Customer.Name))
            {
                _logger.LogError("Customer name is required");
                return false;
            }

            // Validate quantities match
            if (orderTransaction.Supplier.Quantity != orderTransaction.Customer.Quantity)
            {
                _logger.LogError("Supplier and customer quantities must match. Supplier: {SupplierQty}, Customer: {CustomerQty}", 
                    orderTransaction.Supplier.Quantity, orderTransaction.Customer.Quantity);
                return false;
            }

            // Validate prices match
            if (orderTransaction.Supplier.Price != orderTransaction.Customer.Price)
            {
                _logger.LogError("Supplier and customer prices must match. Supplier: {SupplierPrice}, Customer: {CustomerPrice}", 
                    orderTransaction.Supplier.Price, orderTransaction.Customer.Price);
                return false;
            }

            // Validate quantity is positive
            if (orderTransaction.Supplier.Quantity <= 0)
            {
                _logger.LogError("Quantity must be greater than 0. Current: {Quantity}", orderTransaction.Supplier.Quantity);
                return false;
            }

            // Validate price is within expected range (200-1000)
            if (orderTransaction.Supplier.Price < 200 || orderTransaction.Supplier.Price > 1000)
            {
                _logger.LogError("Price must be between 200 and 1000. Current: {Price}", orderTransaction.Supplier.Price);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating order transaction");
            return false;
        }
    }

    private bool ValidateOrderCancellation(OrderCancellationSchema orderCancellation)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(orderCancellation.Customer))
            {
                _logger.LogError("Customer name is required for cancellation");
                return false;
            }

            if (string.IsNullOrWhiteSpace(orderCancellation.Supplier))
            {
                _logger.LogError("Supplier name is required for cancellation");
                return false;
            }

            // Validate quantity is positive
            if (orderCancellation.Quantity <= 0)
            {
                _logger.LogError("Quantity must be greater than 0 for cancellation. Current: {Quantity}", orderCancellation.Quantity);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating order cancellation");
            return false;
        }
    }

    private async Task ProcessOrderTransaction(OrderTransactionSchema orderTransaction, string filePath)
    {
        try
        {
            _logger.LogInformation("Processing validated order transaction:");
            _logger.LogInformation("  Supplier: {SupplierName} - Quantity: {Quantity}, Price: {Price}", 
                orderTransaction.Supplier.Name, orderTransaction.Supplier.Quantity, orderTransaction.Supplier.Price);
            _logger.LogInformation("  Customer: {CustomerName} - Quantity: {Quantity}, Price: {Price}", 
                orderTransaction.Customer.Name, orderTransaction.Customer.Quantity, orderTransaction.Customer.Price);

            // Here you can add specific business logic such as:
            // 1. Create order in database
            // 2. Update inventory
            // 3. Send notifications
            // 4. Generate invoices
            
            _logger.LogInformation("Order transaction processed successfully from file: {FilePath}", filePath);

            // Archive the processed file
            await ArchiveProcessedFile(filePath, FileProcessingQueue.OrderTransaction);
            
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order transaction from {FilePath}", filePath);
            return;
        }
    }

    private async Task ProcessOrderCancellation(OrderCancellationSchema orderCancellation, string filePath)
    {
        try
        {
            _logger.LogInformation("Processing validated order cancellation:");
            _logger.LogInformation("  Customer: {Customer}", orderCancellation.Customer);
            _logger.LogInformation("  Supplier: {Supplier}", orderCancellation.Supplier);
            _logger.LogInformation("  Quantity: {Quantity}", orderCancellation.Quantity);

            // Here you can add specific business logic such as:
            // 1. Find and cancel existing orders
            // 2. Update inventory
            // 3. Process refunds
            // 4. Send cancellation notifications
            
            _logger.LogInformation("Order cancellation processed successfully from file: {FilePath}", filePath);

            // Archive the processed file
            await ArchiveProcessedFile(filePath, FileProcessingQueue.OrderCancellation);
            
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order cancellation from {FilePath}", filePath);
            return;
        }
    }

    private static bool IsFileLocked(string filePath)
    {
        try
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return false;
        }
        catch (IOException)
        {
            return true;
        }
    }

    private Task ArchiveProcessedFile(string filePath, FileProcessingQueue queueType)
    {
        try
        {
            // Create main archive folder
            var archiveFolder = Path.Combine(_fullFolderPath, "_Archive");
            FileHelper.EnsureDirectoryExists(archiveFolder, _logger);
            
            // Create specific subfolder based on queue type
            string subFolder = queueType switch
            {
                FileProcessingQueue.OrderTransaction => "_NewOrders",
                FileProcessingQueue.OrderCancellation => "_CancelOrder",
                _ => "Other"
            };
            
            var targetFolder = Path.Combine(archiveFolder, subFolder);
            FileHelper.EnsureDirectoryExists(targetFolder, _logger);
            
            var fileName = Path.GetFileName(filePath);
            var archivedFilePath = Path.Combine(targetFolder, fileName);
            
            // If file already exists in archive, add timestamp
            if (File.Exists(archivedFilePath))
            {
                var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                var extension = Path.GetExtension(fileName);
                var timestamp = DateTime.Now.ToString("_yyyyMMdd_HHmmss_fff");
                archivedFilePath = Path.Combine(targetFolder, $"{nameWithoutExt}{timestamp}{extension}");
            }
            
            File.Move(filePath, archivedFilePath);
            _logger.LogInformation("File archived: {OriginalPath} -> {ArchivedPath}", filePath, archivedFilePath);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving file {FilePath}", filePath);
            return Task.CompletedTask;
        }
    }

    public override void Dispose()
    {
        _transactionQueueSemaphore?.Dispose();
        _cancellationQueueSemaphore?.Dispose();
        base.Dispose();
    }
}
