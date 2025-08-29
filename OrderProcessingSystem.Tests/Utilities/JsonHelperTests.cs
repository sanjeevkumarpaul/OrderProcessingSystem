using OrderProcessingSystem.Utilities.Helpers;
using System.Text.Json;
using Xunit;

namespace OrderProcessingSystem.Tests.Utilities;

public class JsonHelperTests
{
    [Fact]
    public void SerializeToJson_Should_ReturnFormattedJson()
    {
        // Arrange
        var testObject = new { Name = "Test", Value = 123 };

        // Act
        var result = JsonHelper.SerializeToJson(testObject);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("\"Name\": \"Test\"", result);
        Assert.Contains("\"Value\": 123", result);
        // Check if it's indented (formatted)
        Assert.Contains("\n", result);
    }

    [Fact]
    public void SerializeToJson_WithCustomOptions_Should_UseCustomOptions()
    {
        // Arrange
        var testObject = new { Name = "Test", Value = 123 };
        var customOptions = new JsonSerializerOptions { WriteIndented = false };

        // Act
        var result = JsonHelper.SerializeToJson(testObject, customOptions);

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("\n", result); // Should not be indented
    }

    [Fact]
    public void DeserializeFromJson_Should_ReturnObject()
    {
        // Arrange
        var json = "{\"Name\":\"Test\",\"Value\":123}";

        // Act
        var result = JsonHelper.DeserializeFromJson<TestClass>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result!.Name);
        Assert.Equal(123, result.Value);
    }

    [Fact]
    public void DeserializeFromJson_WithInvalidJson_Should_ThrowException()
    {
        // Arrange
        var invalidJson = "invalid json";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonHelper.DeserializeFromJson<TestClass>(invalidJson));
    }

    private class TestClass
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
