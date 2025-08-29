using OrderProcessingSystem.Events.Models;
using System.Text.Json;

namespace OrderProcessingServer.Services;

public class OrderFileService
{
    public async Task CreateOrderTransactionFileAsync(string customerName, string supplierName, int quantity)
    {
        // Generate a random price between 200 and 1000 (validation range)
        var random = new Random();
        var price = Math.Round((decimal)(random.NextDouble() * 800 + 200), 2); // 200-1000 range
        
        // Create OrderTransaction model
        var orderTransaction = new OrderTransactionSchema
        {
            Supplier = new SupplierInfoSchema
            {
                Name = supplierName,
                Quantity = quantity,
                Price = price
            },
            Customer = new CustomerInfoSchema
            {
                Name = customerName,
                Quantity = quantity,
                Price = price
            }
        };
        
        // Serialize to JSON
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonContent = JsonSerializer.Serialize(orderTransaction, jsonOptions);
        
        // Write to file in BlobStorageSimulation folder
        var filePath = Path.Combine("BlobStorageSimulation", "OrderTransaction.json");
        await File.WriteAllTextAsync(filePath, jsonContent);
    }
    
    public async Task CreateOrderCancellationFileAsync(string customerName, string supplierName, int quantity)
    {
        // Create OrderCancellation model
        var orderCancellation = new OrderCancellationSchema
        {
            Customer = customerName,
            Supplier = supplierName,
            Quantity = quantity
        };
        
        // Serialize to JSON
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonContent = JsonSerializer.Serialize(orderCancellation, jsonOptions);
        
        // Write to file in BlobStorageSimulation folder
        var filePath = Path.Combine("BlobStorageSimulation", "OrderCancellation.json");
        await File.WriteAllTextAsync(filePath, jsonContent);
    }
}
