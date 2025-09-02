namespace OrderProcessingSystem.Core.Configuration;

/// <summary>
/// Configuration class for API endpoints used by client applications
/// </summary>
public class ApiEndpointsConfiguration
{
    public const string SectionName = "ApiEndpoints";

    /// <summary>
    /// Base endpoints for data operations
    /// </summary>
    public DataEndpoints Data { get; set; } = new();

    /// <summary>
    /// Base endpoints for transaction exceptions
    /// </summary>
    public TransExceptionEndpoints TransExceptions { get; set; } = new();

    /// <summary>
    /// Base endpoints for reports
    /// </summary>
    public ReportsEndpoints Reports { get; set; } = new();
}

public class DataEndpoints
{
    public string Orders { get; set; } = "api/data/orders";
    public string Suppliers { get; set; } = "api/data/suppliers";
    public string Customers { get; set; } = "api/data/customers";
    public string CustomersWithOrders { get; set; } = "api/data/customers-with-orders";
    public string SuppliersWithOrders { get; set; } = "api/data/suppliers-with-orders";
}

public class TransExceptionEndpoints
{
    public string Base { get; set; } = "api/transexceptions";
}

public class ReportsEndpoints
{
    public string SalesByCustomer { get; set; } = "api/data/reports/sales-by-customer";
}
