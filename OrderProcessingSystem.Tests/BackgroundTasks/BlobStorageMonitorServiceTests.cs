using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OrderProcessingSystem.Events.FileWatcherTasks;
using OrderProcessingSystem.Events.Models;
using OrderProcessingSystem.Events.Configurations;
using System.Reflection;
using System.Text.Json;
using Xunit;

namespace OrderProcessingSystem.Tests.BackgroundTasks;

public class BlobStorageMonitorServiceTests
{
    private readonly Mock<ILogger<BlobStorageMonitorService>> _mockLogger;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IOptions<BlobStorageSimulationOptions>> _mockOptions;
    private readonly BlobStorageSimulationOptions _options;
    private readonly BlobStorageMonitorService _service;

    public BlobStorageMonitorServiceTests()
    {
        _mockLogger = new Mock<ILogger<BlobStorageMonitorService>>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockOptions = new Mock<IOptions<BlobStorageSimulationOptions>>();
        
        _options = new BlobStorageSimulationOptions
        {
            FolderPath = "./TestData",
            PollingIntervalSeconds = 5,
            MonitoredFileName = "OrderTransaction.json"
        };
        
        _mockOptions.Setup(x => x.Value).Returns(_options);
        
        _service = new BlobStorageMonitorService(
            _mockLogger.Object,
            _mockOptions.Object,
            _mockServiceProvider.Object);
    }

    #region OrderTransaction Validation Tests

    [Fact]
    public void ValidateOrderTransaction_ValidData_ReturnsTrue()
    {
        // Arrange
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "ABC Electronics Ltd",
                Quantity = 25,
                Price = 750.50m
            },
            Customer = new CustomerInfoSchema
            {
                Name = "Tech Solutions Inc",
                Quantity = 25,
                Price = 750.50m
            }
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateOrderTransaction_EmptySupplierName_ReturnsFalse()
    {
        // Arrange
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "", // Empty name
                Quantity = 25,
                Price = 750.50m
            },
            Customer = new CustomerInfoSchema
            {
                Name = "Tech Solutions Inc",
                Quantity = 25,
                Price = 750.50m
            }
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderTransaction_EmptyCustomerName_ReturnsFalse()
    {
        // Arrange
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "ABC Electronics Ltd",
                Quantity = 25,
                Price = 750.50m
            },
            Customer = new CustomerInfoSchema
            {
                Name = string.Empty, // Empty name
                Quantity = 25,
                Price = 750.50m
            }
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderTransaction_MismatchedQuantities_ReturnsFalse()
    {
        // Arrange
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "ABC Electronics Ltd",
                Quantity = 25,
                Price = 750.50m
            },
            Customer = new CustomerInfoSchema
            {
                Name = "Tech Solutions Inc",
                Quantity = 30, // Different quantity
                Price = 750.50m
            }
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderTransaction_MismatchedPrices_ReturnsFalse()
    {
        // Arrange
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "ABC Electronics Ltd",
                Quantity = 25,
                Price = 750.50m
            },
            Customer = new CustomerInfoSchema
            {
                Name = "Tech Solutions Inc",
                Quantity = 25,
                Price = 800.00m // Different price
            }
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderTransaction_ZeroQuantity_ReturnsFalse()
    {
        // Arrange
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "ABC Electronics Ltd",
                Quantity = 0, // Zero quantity
                Price = 750.50m
            },
            Customer = new CustomerInfoSchema
            {
                Name = "Tech Solutions Inc",
                Quantity = 0,
                Price = 750.50m
            }
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderTransaction_NegativeQuantity_ReturnsFalse()
    {
        // Arrange
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "ABC Electronics Ltd",
                Quantity = -5, // Negative quantity
                Price = 750.50m
            },
            Customer = new CustomerInfoSchema
            {
                Name = "Tech Solutions Inc",
                Quantity = -5,
                Price = 750.50m
            }
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderTransaction_PriceBelowMinimum_ReturnsFalse()
    {
        // Arrange
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "ABC Electronics Ltd",
                Quantity = 25,
                Price = 150.00m // Below minimum of 200
            },
            Customer = new CustomerInfoSchema
            {
                Name = "Tech Solutions Inc",
                Quantity = 25,
                Price = 150.00m
            }
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderTransaction_PriceAboveMaximum_ReturnsFalse()
    {
        // Arrange
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "ABC Electronics Ltd",
                Quantity = 25,
                Price = 1500.00m // Above maximum of 1000
            },
            Customer = new CustomerInfoSchema
            {
                Name = "Tech Solutions Inc",
                Quantity = 25,
                Price = 1500.00m
            }
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderTransaction_PriceAtMinimum_ReturnsTrue()
    {
        // Arrange
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "ABC Electronics Ltd",
                Quantity = 25,
                Price = 200.00m // Exactly at minimum
            },
            Customer = new CustomerInfoSchema
            {
                Name = "Tech Solutions Inc",
                Quantity = 25,
                Price = 200.00m
            }
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateOrderTransaction_PriceAtMaximum_ReturnsTrue()
    {
        // Arrange
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "ABC Electronics Ltd",
                Quantity = 25,
                Price = 1000.00m // Exactly at maximum
            },
            Customer = new CustomerInfoSchema
            {
                Name = "Tech Solutions Inc",
                Quantity = 25,
                Price = 1000.00m
            }
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region OrderCancellation Validation Tests

    [Fact]
    public void ValidateOrderCancellation_ValidData_ReturnsTrue()
    {
        // Arrange
        var orderCancellation = new OrderCancellationSchema
        {
            Customer = "Tech Solutions Inc",
            Supplier = "ABC Electronics Ltd",
            Quantity = 15
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderCancellation", orderCancellation);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidateOrderCancellation_EmptyCustomerName_ReturnsFalse()
    {
        // Arrange
        var orderCancellation = new OrderCancellationSchema
        {
            Customer = "", // Empty customer name
            Supplier = "ABC Electronics Ltd",
            Quantity = 15
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderCancellation", orderCancellation);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderCancellation_NullCustomerName_ReturnsFalse()
    {
        // Arrange
        var orderCancellation = new OrderCancellationSchema
        {
            Customer = string.Empty, // Empty customer name
            Supplier = "ABC Electronics Ltd",
            Quantity = 15
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderCancellation", orderCancellation);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderCancellation_EmptySupplierName_ReturnsFalse()
    {
        // Arrange
        var orderCancellation = new OrderCancellationSchema
        {
            Customer = "Tech Solutions Inc",
            Supplier = "", // Empty supplier name
            Quantity = 15
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderCancellation", orderCancellation);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderCancellation_NullSupplierName_ReturnsFalse()
    {
        // Arrange
        var orderCancellation = new OrderCancellationSchema
        {
            Customer = "Tech Solutions Inc",
            Supplier = string.Empty, // Empty supplier name
            Quantity = 15
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderCancellation", orderCancellation);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderCancellation_ZeroQuantity_ReturnsFalse()
    {
        // Arrange
        var orderCancellation = new OrderCancellationSchema
        {
            Customer = "Tech Solutions Inc",
            Supplier = "ABC Electronics Ltd",
            Quantity = 0 // Zero quantity
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderCancellation", orderCancellation);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ValidateOrderCancellation_NegativeQuantity_ReturnsFalse()
    {
        // Arrange
        var orderCancellation = new OrderCancellationSchema
        {
            Customer = "Tech Solutions Inc",
            Supplier = "ABC Electronics Ltd",
            Quantity = -10 // Negative quantity
        };

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderCancellation", orderCancellation);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region JSON Serialization Tests

    [Fact]
    public void OrderTransactionSchema_SerializeDeserialize_MaintainsData()
    {
        // Arrange
        var original = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = "ABC Electronics Ltd",
                Quantity = 25,
                Price = 750.50m
            },
            Customer = new CustomerInfoSchema
            {
                Name = "Tech Solutions Inc",
                Quantity = 25,
                Price = 750.50m
            }
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<OrderTransactionSchema>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.Supplier.Name, deserialized.Supplier.Name);
        Assert.Equal(original.Supplier.Quantity, deserialized.Supplier.Quantity);
        Assert.Equal(original.Supplier.Price, deserialized.Supplier.Price);
        Assert.Equal(original.Customer.Name, deserialized.Customer.Name);
        Assert.Equal(original.Customer.Quantity, deserialized.Customer.Quantity);
        Assert.Equal(original.Customer.Price, deserialized.Customer.Price);
    }

    [Fact]
    public void OrderCancellationSchema_SerializeDeserialize_MaintainsData()
    {
        // Arrange
        var original = new OrderCancellationSchema
        {
            Customer = "Tech Solutions Inc",
            Supplier = "ABC Electronics Ltd",
            Quantity = 15
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<OrderCancellationSchema>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.Customer, deserialized.Customer);
        Assert.Equal(original.Supplier, deserialized.Supplier);
        Assert.Equal(original.Quantity, deserialized.Quantity);
    }

    [Fact]
    public void OrderTransactionSchema_DeserializeInvalidJson_ThrowsJsonException()
    {
        // Arrange
        var invalidJson = @"{
            ""Supplier"": {
                ""Name"": ""ABC Electronics Ltd"",
                ""Quantity"": ""not a number"",
                ""Price"": 750.50
            },
            ""Customer"": {
                ""Name"": ""Tech Solutions Inc"",
                ""Quantity"": 25,
                ""Price"": 750.50
            }
        }";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<OrderTransactionSchema>(invalidJson));
    }

    [Fact]
    public void OrderCancellationSchema_DeserializeInvalidJson_ThrowsJsonException()
    {
        // Arrange
        var invalidJson = @"{
            ""Customer"": ""Tech Solutions Inc"",
            ""Supplier"": ""ABC Electronics Ltd"",
            ""Quantity"": ""not a number""
        }";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<OrderCancellationSchema>(invalidJson));
    }

    #endregion

    #region Schema Compliance Tests

    [Theory]
    [InlineData(@"{""Supplier"":{""Name"":""Test Supplier"",""Quantity"":10,""Price"":500},""Customer"":{""Name"":""Test Customer"",""Quantity"":10,""Price"":500}}", true)]
    [InlineData(@"{""Supplier"":{""Name"":""Test Supplier"",""Quantity"":10,""Price"":500},""Customer"":{""Name"":""Test Customer"",""Quantity"":20,""Price"":500}}", false)]
    [InlineData(@"{""Supplier"":{""Name"":""Test Supplier"",""Quantity"":10,""Price"":500},""Customer"":{""Name"":""Test Customer"",""Quantity"":10,""Price"":600}}", false)]
    [InlineData(@"{""Supplier"":{""Name"":"""",""Quantity"":10,""Price"":500},""Customer"":{""Name"":""Test Customer"",""Quantity"":10,""Price"":500}}", false)]
    [InlineData(@"{""Supplier"":{""Name"":""Test Supplier"",""Quantity"":0,""Price"":500},""Customer"":{""Name"":""Test Customer"",""Quantity"":0,""Price"":500}}", false)]
    [InlineData(@"{""Supplier"":{""Name"":""Test Supplier"",""Quantity"":10,""Price"":100},""Customer"":{""Name"":""Test Customer"",""Quantity"":10,""Price"":100}}", false)]
    [InlineData(@"{""Supplier"":{""Name"":""Test Supplier"",""Quantity"":10,""Price"":1500},""Customer"":{""Name"":""Test Customer"",""Quantity"":10,""Price"":1500}}", false)]
    public void ValidateOrderTransaction_JsonString_ReturnsExpectedResult(string jsonString, bool expectedResult)
    {
        // Arrange
        var orderTransaction = JsonSerializer.Deserialize<OrderTransactionSchema>(jsonString);

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderTransaction", orderTransaction!);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(@"{""Customer"":""Test Customer"",""Supplier"":""Test Supplier"",""Quantity"":10}", true)]
    [InlineData(@"{""Customer"":"""",""Supplier"":""Test Supplier"",""Quantity"":10}", false)]
    [InlineData(@"{""Customer"":""Test Customer"",""Supplier"":"""",""Quantity"":10}", false)]
    [InlineData(@"{""Customer"":""Test Customer"",""Supplier"":""Test Supplier"",""Quantity"":0}", false)]
    [InlineData(@"{""Customer"":""Test Customer"",""Supplier"":""Test Supplier"",""Quantity"":-5}", false)]
    public void ValidateOrderCancellation_JsonString_ReturnsExpectedResult(string jsonString, bool expectedResult)
    {
        // Arrange
        var orderCancellation = JsonSerializer.Deserialize<OrderCancellationSchema>(jsonString);

        // Act
        var result = InvokePrivateMethod<bool>("ValidateOrderCancellation", orderCancellation!);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Helper method to invoke private methods on the service for testing
    /// </summary>
    private T InvokePrivateMethod<T>(string methodName, params object[] parameters)
    {
        var method = typeof(BlobStorageMonitorService)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        
        Assert.NotNull(method);
        
        var result = method.Invoke(_service, parameters);
        return (T)result!;
    }

    #endregion
}
