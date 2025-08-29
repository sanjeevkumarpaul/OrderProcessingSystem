namespace OrderProcessingSystem.Utilities.Helpers;

/// <summary>
/// Utility class for random number generation operations
/// </summary>
public static class RandomHelper
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// Generates a random decimal within a specified range
    /// </summary>
    /// <param name="min">Minimum value (inclusive)</param>
    /// <param name="max">Maximum value (inclusive)</param>
    /// <param name="decimalPlaces">Number of decimal places to round to</param>
    /// <returns>Random decimal value within the specified range</returns>
    public static decimal GenerateRandomDecimal(decimal min, decimal max, int decimalPlaces = 2)
    {
        if (min >= max)
        {
            throw new ArgumentException("Min value must be less than max value");
        }

        var range = (double)(max - min);
        var randomValue = (decimal)(_random.NextDouble() * range) + min;
        return Math.Round(randomValue, decimalPlaces);
    }

    /// <summary>
    /// Generates a random price within the validation range (200-1000)
    /// </summary>
    /// <returns>Random price between 200 and 1000 with 2 decimal places</returns>
    public static decimal GenerateRandomPrice()
    {
        return GenerateRandomDecimal(200m, 1000m, 2);
    }

    /// <summary>
    /// Generates a random integer within a specified range
    /// </summary>
    /// <param name="min">Minimum value (inclusive)</param>
    /// <param name="max">Maximum value (exclusive)</param>
    /// <returns>Random integer within the specified range</returns>
    public static int GenerateRandomInt(int min, int max)
    {
        return _random.Next(min, max);
    }

    /// <summary>
    /// Generates a random double within a specified range
    /// </summary>
    /// <param name="min">Minimum value (inclusive)</param>
    /// <param name="max">Maximum value (inclusive)</param>
    /// <returns>Random double within the specified range</returns>
    public static double GenerateRandomDouble(double min, double max)
    {
        if (min >= max)
        {
            throw new ArgumentException("Min value must be less than max value");
        }

        return _random.NextDouble() * (max - min) + min;
    }
}
