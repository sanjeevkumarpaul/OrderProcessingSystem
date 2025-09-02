namespace OrderProcessingSystem.Contracts.Interfaces;

/// <summary>
/// Interface for creating order-related JSON files in the BlobStorageSimulation folder
/// </summary>
public interface IOrderFileService
{
    /// <summary>
    /// Creates an order transaction JSON file in the BlobStorageSimulation folder
    /// </summary>
    /// <param name="customerName">Name of the customer</param>
    /// <param name="supplierName">Name of the supplier</param>
    /// <param name="quantity">Order quantity</param>
    Task CreateOrderTransactionFileAsync(string customerName, string supplierName, int quantity);

    /// <summary>
    /// Creates an order cancellation JSON file in the BlobStorageSimulation folder
    /// </summary>
    /// <param name="customerName">Name of the customer</param>
    /// <param name="supplierName">Name of the supplier</param>
    /// <param name="quantity">Order quantity to cancel</param>
    Task CreateOrderCancellationFileAsync(string customerName, string supplierName, int quantity);
}
