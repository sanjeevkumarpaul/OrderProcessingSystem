using Microsoft.Extensions.Options;
using System.Text.Json;

namespace OrderProcessingServer.BackgroundTasks;

public class BlobStorageMonitorService : BackgroundService
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
                await CheckForOrderTransactionFiles();
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
                Filter = _options.MonitoredFileName,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            _fileWatcher.Created += OnFileCreated;
            _fileWatcher.Changed += OnFileChanged;
            
            _logger.LogInformation("File system watcher setup complete for {FileName} in {FolderPath}", 
                _options.MonitoredFileName, _fullFolderPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup file system watcher");
        }
    }

    private async void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File created: {FilePath}", e.FullPath);
        await ProcessOrderTransactionFile(e.FullPath);
    }

    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("File changed: {FilePath}", e.FullPath);
        await ProcessOrderTransactionFile(e.FullPath);
    }

    private async Task CheckForOrderTransactionFiles()
    {
        try
        {
            var filePath = Path.Combine(_fullFolderPath, _options.MonitoredFileName);
            
            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                _logger.LogDebug("Found {FileName} - Size: {Size} bytes, Last Modified: {LastModified}",
                    _options.MonitoredFileName, fileInfo.Length, fileInfo.LastWriteTime);
                
                await ProcessOrderTransactionFile(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for order transaction files");
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

            // Try to parse JSON to validate format
            try
            {
                using var jsonDocument = JsonDocument.Parse(fileContent);
                _logger.LogInformation("Successfully parsed JSON from {FilePath}", filePath);
                
                // Here you can add logic to process the order transaction
                // For example, you might want to:
                // 1. Validate the order data
                // 2. Save it to database
                // 3. Send notifications
                // 4. Update order status
                
                await ProcessOrderTransaction(jsonDocument, filePath);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid JSON format in file {FilePath}", filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file {FilePath}", filePath);
        }
    }

    private Task ProcessOrderTransaction(JsonDocument orderTransaction, string filePath)
    {
        try
        {
            // Extract order information
            var root = orderTransaction.RootElement;
            
            if (root.TryGetProperty("orderId", out var orderIdElement))
            {
                var orderId = orderIdElement.GetString();
                _logger.LogInformation("Processing order transaction for Order ID: {OrderId}", orderId);
            }

            if (root.TryGetProperty("transactionType", out var transactionTypeElement))
            {
                var transactionType = transactionTypeElement.GetString();
                _logger.LogInformation("Transaction type: {TransactionType}", transactionType);
            }

            // Here you can add specific business logic based on your needs
            // For now, we'll just log the successful processing
            _logger.LogInformation("Order transaction processed successfully from file: {FilePath}", filePath);

            // Optionally, you might want to move or delete the processed file
            // await ArchiveProcessedFile(filePath);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order transaction from {FilePath}", filePath);
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
