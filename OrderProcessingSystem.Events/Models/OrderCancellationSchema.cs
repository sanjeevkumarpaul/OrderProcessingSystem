using System.Text.Json.Serialization;

namespace OrderProcessingSystem.Events.Models;

public class OrderCancellationSchema
{
    [JsonPropertyName("Customer")]
    public string Customer { get; set; } = string.Empty;

    [JsonPropertyName("Supplier")]
    public string Supplier { get; set; } = string.Empty;

    [JsonPropertyName("Quantity")]
    public int Quantity { get; set; }
}
