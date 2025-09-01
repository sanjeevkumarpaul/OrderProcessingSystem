using Microsoft.Extensions.Options;
using OrderProcessingSystem.Core.Configuration;
using Xunit;

namespace OrderProcessingSystem.Tests.Configuration;

public class BlobStorageSimulationOptionsTests
{
    [Fact]
    public void ApiEndpoints_IsValid_ReturnsTrueWhenAllPropertiesSet()
    {
        // Arrange
        var apiEndpoints = new ApiEndpoints
        {
            BaseUrl = "http://localhost:5269/api",
            OrderTransactionEndpoint = "orderprocessing/process-order-transaction",
            OrderCancellationEndpoint = "orderprocessing/process-order-cancellation",
            TransExceptionsEndpoint = "TransExceptions"
        };

        // Act
        var isValid = apiEndpoints.IsValid();

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ApiEndpoints_IsValid_ReturnsFalseWhenBaseUrlMissing()
    {
        // Arrange
        var apiEndpoints = new ApiEndpoints
        {
            BaseUrl = "", // Missing
            OrderTransactionEndpoint = "orderprocessing/process-order-transaction",
            OrderCancellationEndpoint = "orderprocessing/process-order-cancellation",
            TransExceptionsEndpoint = "TransExceptions"
        };

        // Act
        var isValid = apiEndpoints.IsValid();

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ApiEndpoints_GetValidationErrors_ReturnsErrorsForMissingProperties()
    {
        // Arrange
        var apiEndpoints = new ApiEndpoints
        {
            BaseUrl = "", // Missing
            OrderTransactionEndpoint = "orderprocessing/process-order-transaction",
            OrderCancellationEndpoint = "", // Missing
            TransExceptionsEndpoint = "TransExceptions"
        };

        // Act
        var errors = apiEndpoints.GetValidationErrors().ToList();

        // Assert
        Assert.Contains("BaseUrl is required in ApiEndpoints configuration", errors);
        Assert.Contains("OrderCancellationEndpoint is required in ApiEndpoints configuration", errors);
        Assert.Equal(2, errors.Count);
    }

    [Fact]
    public void ApiEndpoints_UrlProperties_BuildCorrectUrls()
    {
        // Arrange
        var apiEndpoints = new ApiEndpoints
        {
            BaseUrl = "http://localhost:5269/api",
            OrderTransactionEndpoint = "orderprocessing/process-order-transaction",
            OrderCancellationEndpoint = "orderprocessing/process-order-cancellation",
            TransExceptionsEndpoint = "TransExceptions"
        };

        // Act & Assert
        Assert.Equal("http://localhost:5269/api/orderprocessing/process-order-transaction", 
            apiEndpoints.OrderTransactionUrl);
        Assert.Equal("http://localhost:5269/api/orderprocessing/process-order-cancellation", 
            apiEndpoints.OrderCancellationUrl);
        Assert.Equal("http://localhost:5269/api/TransExceptions", 
            apiEndpoints.TransExceptionsUrl);
    }

    [Fact]
    public void ApiEndpoints_UrlProperties_HandleSlashesCorrectly()
    {
        // Arrange
        var apiEndpoints = new ApiEndpoints
        {
            BaseUrl = "http://localhost:5269/api/", // Trailing slash
            OrderTransactionEndpoint = "/orderprocessing/process-order-transaction", // Leading slash
            OrderCancellationEndpoint = "orderprocessing/process-order-cancellation", // No slash
            TransExceptionsEndpoint = "/TransExceptions/" // Both slashes
        };

        // Act & Assert
        Assert.Equal("http://localhost:5269/api/orderprocessing/process-order-transaction", 
            apiEndpoints.OrderTransactionUrl);
        Assert.Equal("http://localhost:5269/api/orderprocessing/process-order-cancellation", 
            apiEndpoints.OrderCancellationUrl);
        Assert.Equal("http://localhost:5269/api/TransExceptions/", 
            apiEndpoints.TransExceptionsUrl);
    }
}
