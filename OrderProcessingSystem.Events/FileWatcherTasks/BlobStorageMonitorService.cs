using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderProcessingSystem.Contracts.Interfaces;
using OrderProcessingSystem.Events.Configurations;
using OrderProcessingSystem.Events.Models;
using System.Text.Json;

namespace OrderProcessingSystem.Events.FileWatcherTasks;

public class BlobStorageMonitorService : BackgroundService, IBlobStorageMonitorService
{
    private readonly ILogger<BlobStorageMonitorService> _logger;
    private readonly BlobStorageSimulationOptions _options;
    private readonly IServiceProvider _serviceProvider;
    private FileSystemWatcher? _fileWatcher;
    private readonly string _fullFolderPath;

    public BlobStorageMonitorService(
        ILogger<BlobStorageMonitorService> logger,
        IOptions<BlobStorageSimulationOptions> options,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _options = options.Value;
        _serviceProvider = serviceProvider;
        
        // Convert relative path to absolute path
        _fullFolderPath = Path.GetFullPath(_options.FolderPath);
        
        _logger.LogInformation("BlobStorageMonitorService initialized. Monitoring folder: {FolderPath}", _fullFolderPath);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BlobStorageMonitorService started");

        // Ensure the directory exists
        if (!Directory.Exists(_fullFolderPath))
        {
            _logger.LogInformation("Creating directory: {FolderPath}", _fullFolderPath);
            Directory.CreateDirectory(_fullFolderPath);
        }

        // Set up file system watcher
        SetupFileWatcher();

        // Also do periodic polling as backup
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckForJsonFiles();
                await Task.Delay(TimeSpan.FromSeconds(_options.PollingIntervalSeconds), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during periodic file check");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Wait before retrying
            }
        }

        _logger.LogInformation("BlobStorageMonitorService stopped");
    }

    private void SetupFileWatcher()
    {
        try
        {
            _fileWatcher = new FileSystemWatcher(_fullFolderPath)
            {
                Filter = "*.json", // Monitor all JSON files
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            _fileWatcher.Created += OnFileCreated;
            _fileWatcher.Changed += OnFileChanged;
            
            _logger.LogInformation("File system watcher setup complete for JSON files in {FolderPath}", _fullFolderPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup file system watcher");
        }
    }

    private async void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File created: {FilePath}", e.FullPath);
        await ProcessJsonFile(e.FullPath);
    }

    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File changed: {FilePath}", e.FullPath);
        await ProcessJsonFile(e.FullPath);
    }

    private async Task CheckForJsonFiles()
    {
        try
        {
            var jsonFiles = Directory.GetFiles(_fullFolderPath, "*.json");
            
            foreach (var filePath in jsonFiles)
            {
                var fileName = Path.GetFileName(filePath);
                _logger.LogDebug("Found JSON file: {FileName}", fileName);
                
                await ProcessJsonFile(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for JSON files");
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

    private Task ProcessOrderTransaction(OrderTransactionSchema orderTransaction, string filePath)
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

            // Optionally, archive or delete the processed file
            // await ArchiveProcessedFile(filePath);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order transaction from {FilePath}", filePath);
            return Task.CompletedTask;
        }
    }

    private Task ProcessOrderCancellation(OrderCancellationSchema orderCancellation, string filePath)
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

            // Optionally, archive or delete the processed file
            // await ArchiveProcessedFile(filePath);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order cancellation from {FilePath}", filePath);
            return Task.CompletedTask;
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

    private Task ArchiveProcessedFile(string filePath)
    {
        try
        {
            var archiveFolder = Path.Combine(_fullFolderPath, "processed");
            Directory.CreateDirectory(archiveFolder);
            
            var fileName = Path.GetFileName(filePath);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var archivedFileName = $"{timestamp}_{fileName}";
            var archivedFilePath = Path.Combine(archiveFolder, archivedFileName);
            
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
        _fileWatcher?.Dispose();
        base.Dispose();
    }
}
