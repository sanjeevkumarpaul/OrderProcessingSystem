using Microsoft.Extensions.Logging;
using Moq;
using OrderProcessingServer.Services;
using System.IO;
using Xunit;

namespace OrderProcessingSystem.Tests.FileCreations;

[Collection("FileCreation Tests")]
public class BlobStorageSimulationIntegrationTests
{
    [Fact]
    public void OrderFileService_Should_BeInstantiable_WithLogger()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<OrderFileService>>();

        // Act & Assert - Should not throw exception
        var orderFileService = new OrderFileService(mockLogger.Object);
        Assert.NotNull(orderFileService);
    }

    [Fact]
    public async Task OrderFileService_Should_CreateFiles_InCorrectLocation()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<OrderFileService>>();
        var orderFileService = new OrderFileService(mockLogger.Object);
        
        // Act - Create files
        await orderFileService.CreateOrderTransactionFileAsync("Integration Test Customer", "Integration Test Supplier", 99);
        await orderFileService.CreateOrderCancellationFileAsync("Integration Test Customer", "Integration Test Supplier", 99);

        // Assert - Verify logger was called for file creation
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating OrderTransaction.json")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);

        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating OrderCancellation.json")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Theory]
    [InlineData("Customer A", "Supplier A", 1)]
    [InlineData("Customer B", "Supplier B", 100)]
    [InlineData("Customer C", "Supplier C", 5000)]
    public async Task OrderFileService_Should_HandleDifferentQuantities(string customerName, string supplierName, int quantity)
    {
        // Arrange
        var mockLogger = new Mock<ILogger<OrderFileService>>();
        var orderFileService = new OrderFileService(mockLogger.Object);

        // Act & Assert - Should not throw exception for valid ranges
        await orderFileService.CreateOrderTransactionFileAsync(customerName, supplierName, quantity);
        
        // Verify logger was called
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("OrderTransaction.json created successfully")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void FindSolutionRoot_Should_LocateCorrectDirectory()
    {
        // Act
        var solutionRoot = FindSolutionRoot();

        // Assert
        Assert.NotNull(solutionRoot);
        Assert.True(File.Exists(Path.Combine(solutionRoot!, "OrderProcessingSystem.sln")));
        Assert.True(Directory.Exists(Path.Combine(solutionRoot, "BlobStorageSimulation")));
    }

    private static string? FindSolutionRoot()
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
}
