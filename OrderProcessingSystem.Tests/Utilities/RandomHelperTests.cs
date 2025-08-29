using OrderProcessingSystem.Utilities.Helpers;
using Xunit;

namespace OrderProcessingSystem.Tests.Utilities;

public class RandomHelperTests
{
    [Fact]
    public void GenerateRandomDecimal_Should_ReturnValueWithinRange()
    {
        // Arrange
        var min = 10m;
        var max = 20m;

        // Act
        var result = RandomHelper.GenerateRandomDecimal(min, max);

        // Assert
        Assert.True(result >= min && result <= max);
    }

    [Fact]
    public void GenerateRandomDecimal_Should_RespectDecimalPlaces()
    {
        // Arrange
        var min = 10m;
        var max = 20m;
        var decimalPlaces = 3;

        // Act
        var result = RandomHelper.GenerateRandomDecimal(min, max, decimalPlaces);

        // Assert
        var decimalPart = result - Math.Truncate(result);
        var actualDecimalPlaces = decimalPart == 0 ? 0 : BitConverter.GetBytes(decimal.GetBits(result)[3])[2];
        Assert.True(actualDecimalPlaces <= decimalPlaces);
    }

    [Fact]
    public void GenerateRandomDecimal_WithInvalidRange_Should_ThrowException()
    {
        // Arrange
        var min = 20m;
        var max = 10m; // Invalid: min > max

        // Act & Assert
        Assert.Throws<ArgumentException>(() => RandomHelper.GenerateRandomDecimal(min, max));
    }

    [Fact]
    public void GenerateRandomPrice_Should_ReturnPriceInValidRange()
    {
        // Act
        var result = RandomHelper.GenerateRandomPrice();

        // Assert
        Assert.True(result >= 200m && result <= 1000m);
        Assert.Equal(2, BitConverter.GetBytes(decimal.GetBits(result)[3])[2]); // Check 2 decimal places
    }

    [Fact]
    public void GenerateRandomInt_Should_ReturnValueWithinRange()
    {
        // Arrange
        var min = 5;
        var max = 15;

        // Act
        var result = RandomHelper.GenerateRandomInt(min, max);

        // Assert
        Assert.True(result >= min && result < max); // max is exclusive for int
    }

    [Fact]
    public void GenerateRandomDouble_Should_ReturnValueWithinRange()
    {
        // Arrange
        var min = 1.5;
        var max = 3.7;

        // Act
        var result = RandomHelper.GenerateRandomDouble(min, max);

        // Assert
        Assert.True(result >= min && result <= max);
    }

    [Fact]
    public void GenerateRandomDouble_WithInvalidRange_Should_ThrowException()
    {
        // Arrange
        var min = 5.0;
        var max = 3.0; // Invalid: min > max

        // Act & Assert
        Assert.Throws<ArgumentException>(() => RandomHelper.GenerateRandomDouble(min, max));
    }

    [Fact]
    public void GenerateRandomMethods_Should_ProduceDifferentValues()
    {
        // Act - Generate multiple values
        var values = new List<decimal>();
        for (int i = 0; i < 10; i++)
        {
            values.Add(RandomHelper.GenerateRandomPrice());
        }

        // Assert - Should have some variation (not all identical)
        var uniqueValues = values.Distinct().Count();
        Assert.True(uniqueValues > 1, "Random generator should produce varied results");
    }
}
