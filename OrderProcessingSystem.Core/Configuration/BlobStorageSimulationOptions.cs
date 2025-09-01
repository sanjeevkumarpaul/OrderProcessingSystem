namespace OrderProcessingSystem.Core.Configuration;

public class BlobStorageSimulationOptions
{
    public const string SectionName = "BlobStorageSimulation";
    
    public string FolderPath { get; set; } = string.Empty;
    public string MonitoredFileName { get; set; } = "OrderTransaction.json";
    public int PollingIntervalSeconds { get; set; } = 5;
    
    /// <summary>
    /// Configuration for API endpoints used by the background service
    /// </summary>
    public ApiEndpoints ApiEndpoints { get; set; } = new();
}

public class ApiEndpoints
{
    /// <summary>
    /// Base URL for the Order Processing API
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Endpoint for processing order transactions
    /// </summary>
    public string OrderTransactionEndpoint { get; set; } = string.Empty;
    
    /// <summary>
    /// Endpoint for processing order cancellations
    /// </summary>
    public string OrderCancellationEndpoint { get; set; } = string.Empty;
    
    /// <summary>
    /// Endpoint for creating transaction exceptions
    /// </summary>
    public string TransExceptionsEndpoint { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the full URL for order transaction processing
    /// </summary>
    public string OrderTransactionUrl => $"{BaseUrl.TrimEnd('/')}/{OrderTransactionEndpoint.TrimStart('/')}";
    
    /// <summary>
    /// Gets the full URL for order cancellation processing
    /// </summary>
    public string OrderCancellationUrl => $"{BaseUrl.TrimEnd('/')}/{OrderCancellationEndpoint.TrimStart('/')}";
    
    /// <summary>
    /// Gets the full URL for creating transaction exceptions
    /// </summary>
    public string TransExceptionsUrl => $"{BaseUrl.TrimEnd('/')}/{TransExceptionsEndpoint.TrimStart('/')}";
    
    /// <summary>
    /// Validates that all required endpoint configurations are provided
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(BaseUrl) &&
               !string.IsNullOrWhiteSpace(OrderTransactionEndpoint) &&
               !string.IsNullOrWhiteSpace(OrderCancellationEndpoint) &&
               !string.IsNullOrWhiteSpace(TransExceptionsEndpoint);
    }
    
    /// <summary>
    /// Gets validation error messages for missing configuration
    /// </summary>
    public IEnumerable<string> GetValidationErrors()
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(BaseUrl))
            errors.Add("BaseUrl is required in ApiEndpoints configuration");
            
        if (string.IsNullOrWhiteSpace(OrderTransactionEndpoint))
            errors.Add("OrderTransactionEndpoint is required in ApiEndpoints configuration");
            
        if (string.IsNullOrWhiteSpace(OrderCancellationEndpoint))
            errors.Add("OrderCancellationEndpoint is required in ApiEndpoints configuration");
            
        if (string.IsNullOrWhiteSpace(TransExceptionsEndpoint))
            errors.Add("TransExceptionsEndpoint is required in ApiEndpoints configuration");
            
        return errors;
    }
}
