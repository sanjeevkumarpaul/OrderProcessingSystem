using System.Text.Json.Serialization;

namespace OrderProcessingServer.BackgroundTasks.Models;

public class OrderTransactionModel
{
    [JsonPropertyName("Supplier")]
    public SupplierInfo Supplier { get; set; } = new();

    [JsonPropertyName("Customer")]
    public CustomerInfo Customer { get; set; } = new();
}

public class SupplierInfo
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("Price")]
    public decimal Price { get; set; }
}

public class CustomerInfo
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("Price")]
    public decimal Price { get; set; }
}
