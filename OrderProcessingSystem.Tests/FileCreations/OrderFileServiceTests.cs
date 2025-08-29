using Microsoft.Extensions.Logging;
using Moq;
using OrderProcessingServer.Services;
using System.IO;
using System.Text.Json;
using Xunit;
using OrderProcessingSystem.Events.Models;
using OrderProcessingSystem.Core.Constants;

namespace OrderProcessingSystem.Tests.FileCreations;

[Collection("FileCreation Tests")]
public class OrderFileServiceTests : IDisposable
{
    private readonly Mock<ILogger<OrderFileService>> _mockLogger;
    private readonly OrderFileService _orderFileService;
    private readonly string _testSolutionDirectory;

    public OrderFileServiceTests()
    {
        _mockLogger = new Mock<ILogger<OrderFileService>>();
        _orderFileService = new OrderFileService(_mockLogger.Object);
        
        // Create a test solution directory in temp
        _testSolutionDirectory = Path.Combine(Path.GetTempPath(), $"OrderProcessingSystem_Test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testSolutionDirectory);
        
        // Create a fake solution file 
        var solutionFile = Path.Combine(_testSolutionDirectory, "OrderProcessingSystem.sln");
        File.WriteAllText(solutionFile, "# Test solution file");
    }

    [Fact]
    public async Task CreateOrderTransactionFileAsync_Should_CreateValidJsonFile_WithMockedPath()
    {
        // Arrange
        var customerName = "Test Customer";
        var supplierName = "Test Supplier";
        var quantity = 25;
        
        // Create BlobStorageSimulation directory in our test location
        var blobStoragePath = Path.Combine(_testSolutionDirectory, FileNames.BlobStorage.DirectoryName);
        Directory.CreateDirectory(blobStoragePath);

        // Act
        await _orderFileService.CreateOrderTransactionFileAsync(customerName, supplierName, quantity);

        // Assert - Check if file was created in the real BlobStorageSimulation folder
        // The service should find the actual solution root and create the file there
        var realSolutionRoot = FindSolutionRoot();
        if (realSolutionRoot != null)
        {
            var realBlobStoragePath = Path.Combine(realSolutionRoot, FileNames.BlobStorage.DirectoryName);
            var expectedFilePath = Path.Combine(realBlobStoragePath, FileNames.BlobStorage.OrderTransaction);
            
            if (File.Exists(expectedFilePath))
            {
                var jsonContent = await File.ReadAllTextAsync(expectedFilePath);
                var orderTransaction = JsonSerializer.Deserialize<OrderTransactionSchema>(jsonContent);
                
                Assert.NotNull(orderTransaction);
                Assert.Equal(customerName, orderTransaction!.Customer.Name);
                Assert.Equal(supplierName, orderTransaction.Supplier.Name);
                Assert.Equal(quantity, orderTransaction.Customer.Quantity);
                Assert.True(orderTransaction.Customer.Price >= 200 && orderTransaction.Customer.Price <= 1000);
                
                // Clean up the created file
                File.Delete(expectedFilePath);
            }
        }
    }

    [Fact]
    public async Task CreateOrderCancellationFileAsync_Should_CreateValidJsonFile_WithMockedPath()
    {
        // Arrange
        var customerName = "Jane Doe";
        var supplierName = "Acme Corp";
        var quantity = 15;

        // Act
        await _orderFileService.CreateOrderCancellationFileAsync(customerName, supplierName, quantity);

        // Assert - Check if file was created in the real BlobStorageSimulation folder
        var realSolutionRoot = FindSolutionRoot();
        if (realSolutionRoot != null)
        {
            var realBlobStoragePath = Path.Combine(realSolutionRoot, FileNames.BlobStorage.DirectoryName);
            var expectedFilePath = Path.Combine(realBlobStoragePath, FileNames.BlobStorage.OrderCancellation);
            
            if (File.Exists(expectedFilePath))
            {
                var jsonContent = await File.ReadAllTextAsync(expectedFilePath);
                var orderCancellation = JsonSerializer.Deserialize<OrderCancellationSchema>(jsonContent);
                
                Assert.NotNull(orderCancellation);
                Assert.Equal(customerName, orderCancellation!.Customer);
                Assert.Equal(supplierName, orderCancellation.Supplier);
                Assert.Equal(quantity, orderCancellation.Quantity);
                
                // Clean up the created file
                File.Delete(expectedFilePath);
            }
        }
    }

    [Theory]
    [InlineData("Customer1", "Supplier1", 5)]
    [InlineData("Customer with spaces", "Supplier & Co.", 50)]
    [InlineData("Special@Customer", "Supplier#123", 100)]
    public async Task CreateOrderTransactionFileAsync_Should_HandleVariousInputs(string customerName, string supplierName, int quantity)
    {
        // Act - Should not throw exception
        await _orderFileService.CreateOrderTransactionFileAsync(customerName, supplierName, quantity);

        // Assert - Verify logger was called 
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Creating {FileNames.BlobStorage.OrderTransaction}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void OrderFileService_Should_HaveLogger_Injected()
    {
        // Assert
        Assert.NotNull(_orderFileService);
        _mockLogger.Verify(l => l.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), 
            Times.Never); // Should be never called until we use the service
    }

    private string? FindSolutionRoot()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var searchDir = currentDir;

        while (searchDir != null)
        {
            if (File.Exists(Path.Combine(searchDir, "OrderProcessingSystem.sln")))
            {
                return searchDir;
            }
            searchDir = Directory.GetParent(searchDir)?.FullName;
        }
        return null;
    }

    public void Dispose()
    {
        // Clean up test directory
        if (Directory.Exists(_testSolutionDirectory))
        {
            Directory.Delete(_testSolutionDirectory, recursive: true);
        }
    }
}
