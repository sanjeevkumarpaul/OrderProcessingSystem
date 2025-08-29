using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Events.FileWatcherTasks;
using OrderProcessingSystem.Core.Enums;
using OrderProcessingSystem.Utilities;
using System.Text.Json;
using OrderProcessingSystem.Events.Models.Schemas;

namespace OrderProcessingSystem.Tests.Integration
{
    [Fact]
    public async Task BlobStorageMonitorService_Should_ArchiveFilesAfterProcessing()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<BlobStorageMonitorService>>();
        var tempFolder = Path.Combine(Path.GetTempPath(), "BlobStorageTest_" + Guid.NewGuid().ToString("N")[..8]);
        
        try
        {
            Directory.CreateDirectory(tempFolder);
            
            var service = new BlobStorageMonitorService(mockLogger.Object, tempFolder);
            
            // Create a valid test file
            var testTransaction = new OrderTransactionSchema
            {
                Supplier = new SupplierInfoSchema { Name = "Test Supplier", Quantity = 10, Price = 500.00m },
                Customer = new CustomerInfoSchema { Name = "Test Customer", Quantity = 10, Price = 500.00m }
            };
            
            var fileName = "OrderTransaction_Test.json";
            var filePath = Path.Combine(tempFolder, fileName);
            var jsonContent = JsonSerializer.Serialize(testTransaction, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, jsonContent);
            
            // Act - Queue the file for processing
            service.QueueFileForProcessing(fileName, FileProcessingQueue.OrderTransaction);
            
            // Give it a moment to process
            await Task.Delay(2000);
            
            // Assert - Check that archive folders exist
            var archiveFolder = Path.Combine(tempFolder, "_Archive");
            var newOrdersFolder = Path.Combine(archiveFolder, "_NewOrders");
            
            Assert.True(Directory.Exists(archiveFolder), "Archive folder should be created");
            Assert.True(Directory.Exists(newOrdersFolder), "_NewOrders folder should be created");
            
            // Check that file was moved to archive
            var archivedFiles = Directory.GetFiles(newOrdersFolder, "*.json");
            Assert.Single(archivedFiles);
            Assert.Contains("OrderTransaction_Test.json", archivedFiles[0]);
            
            // Original file should be gone
            Assert.False(File.Exists(filePath), "Original file should be moved to archive");
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempFolder))
            {
                Directory.Delete(tempFolder, true);
            }
        }
    }
}
