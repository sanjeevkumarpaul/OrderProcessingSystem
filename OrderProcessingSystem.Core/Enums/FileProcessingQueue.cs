namespace OrderProcessingSystem.Core.Enums;

/// <summary>
/// Defines the different queues available for file processing
/// </summary>
public enum FileProcessingQueue
{
    /// <summary>
    /// Queue for processing order transaction files
    /// </summary>
    OrderTransaction,
    
    /// <summary>
    /// Queue for processing order cancellation files
    /// </summary>
    OrderCancellation
}
