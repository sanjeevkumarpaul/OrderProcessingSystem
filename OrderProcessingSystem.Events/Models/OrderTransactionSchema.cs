using System.Text.Json.Serialization;

namespace OrderProcessingSystem.Events.Models;

public class OrderTransactionSchema
{
    [JsonPropertyName("Supplier")]
    public SupplierInfoSchema Supplier { get; set; } = new();

    [JsonPropertyName("Customer")]
    public CustomerInfoSchema Customer { get; set; } = new();
}

public class SupplierInfoSchema
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("Price")]
    public decimal Price { get; set; }
}

public class CustomerInfoSchema
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("Price")]
    public decimal Price { get; set; }
}
