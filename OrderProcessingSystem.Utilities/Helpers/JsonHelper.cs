using System.Text.Json;

namespace OrderProcessingSystem.Utilities.Helpers;

/// <summary>
/// Utility class for JSON serialization operations
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// Serializes an object to JSON with indented formatting
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="obj">The object to serialize</param>
    /// <returns>JSON string representation of the object</returns>
    public static string SerializeToJson<T>(T obj)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        return JsonSerializer.Serialize(obj, jsonOptions);
    }

    /// <summary>
    /// Serializes an object to JSON with custom options
    /// </summary>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <param name="obj">The object to serialize</param>
    /// <param name="options">Custom JSON serialization options</param>
    /// <returns>JSON string representation of the object</returns>
    public static string SerializeToJson<T>(T obj, JsonSerializerOptions options)
    {
        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Deserializes JSON to an object of the specified type
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="json">The JSON string to deserialize</param>
    /// <returns>Deserialized object of type T</returns>
    public static T? DeserializeFromJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// Deserializes JSON to an object of the specified type with custom options
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="json">The JSON string to deserialize</param>
    /// <param name="options">Custom JSON deserialization options</param>
    /// <returns>Deserialized object of type T</returns>
    public static T? DeserializeFromJson<T>(string json, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(json, options);
    }
}
