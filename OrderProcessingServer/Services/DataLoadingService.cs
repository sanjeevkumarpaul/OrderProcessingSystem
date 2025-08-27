using OrderProcessingServer.Shared.Dto;
using System.Net.Http.Json;

namespace OrderProcessingServer.Services;

/// <summary>
/// Service for loading and processing data with common calculations
/// Eliminates duplication across pages for data loading logic
/// </summary>
public class DataLoadingService
{

    private const string _ApiClient = "ApiClient";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DataLoadingService> _logger;

    public DataLoadingService(IHttpClientFactory httpClientFactory, ILogger<DataLoadingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private async Task<(List<T> data, HttpClient client)> GetDataFromApiAsync<T>(string endpoint, HttpClient? client = null) where T : class
    {
        try
        {
            var httpClient = client ?? _httpClientFactory.CreateClient(_ApiClient);
            var data = await httpClient.GetFromJsonAsync<List<T>>(endpoint) ?? new List<T>();
            return (data, httpClient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load data from API endpoint: {Endpoint}", endpoint);
            return (new List<T>(), client ?? _httpClientFactory.CreateClient(_ApiClient));
        }
    }

    /// <summary>
    /// Load customers with calculated OrdersCount and TotalSales
    /// </summary>
    public async Task<List<CustomerDto>> LoadCustomersWithCalculationsAsync()
    {
        try
        {
            _logger.LogInformation("Loading customers with order calculations");

            // Load base data in parallel for better performance, reusing the same client
            var (customers, client) = await GetDataFromApiAsync<CustomerDto>("api/data/customers");
            var (orders, _) = await GetDataFromApiAsync<OrderDto>("api/data/orders", client);

            // Calculate OrdersCount and TotalSales per customer
            var customerGroups = orders
                .GroupBy(o => o.CustomerId)
                .ToDictionary(g => g.Key, g => new { Count = g.Count(), Total = g.Sum(x => x.Total) });

            foreach (var customer in customers)
            {
                if (customerGroups.TryGetValue(customer.CustomerId, out var group))
                {
                    customer.OrdersCount = group.Count;
                    customer.TotalSales = group.Total;
                }
            }

            _logger.LogInformation("Successfully loaded {CustomerCount} customers with calculations", customers.Count);
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load customers with calculations");
            return new List<CustomerDto>();
        }
    }

    /// <summary>
    /// Load suppliers with calculated OrdersSupplied
    /// </summary>
    public async Task<List<SupplierDto>> LoadSuppliersWithCalculationsAsync()
    {
        try
        {
            _logger.LogInformation("Loading suppliers with order calculations");
            
            // Load base data in parallel for better performance, reusing the same client
            var (suppliers, client) = await GetDataFromApiAsync<SupplierDto>("api/data/suppliers");
            var (orders, _) = await GetDataFromApiAsync<OrderDto>("api/data/orders", client);

            // Calculate OrdersSupplied per supplier
            var supplierGroups = orders
                .Where(o => o.SupplierId.HasValue)
                .GroupBy(o => o.SupplierId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var supplier in suppliers)
            {
                if (supplierGroups.TryGetValue(supplier.SupplierId, out var count))
                {
                    supplier.OrdersSupplied = count;
                }
            }

            _logger.LogInformation("Successfully loaded {SupplierCount} suppliers with calculations", suppliers.Count);
            return suppliers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load suppliers with calculations");
            return new List<SupplierDto>();
        }
    }

    /// <summary>
    /// Load basic customer data without calculations (optimized for dropdowns)
    /// </summary>
    public async Task<List<CustomerDto>> LoadCustomersAsync()
    {
        try
        {
            _logger.LogInformation("Loading basic customers data");
            
            var (customers, _) = await GetDataFromApiAsync<CustomerDto>("api/data/customers");

            _logger.LogInformation("Successfully loaded {CustomerCount} customers", customers.Count);
            return customers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load customers");
            return new List<CustomerDto>();
        }
    }

    /// <summary>
    /// Load basic supplier data without calculations (optimized for dropdowns)
    /// </summary>
    public async Task<List<SupplierDto>> LoadSuppliersAsync()
    {
        try
        {
            _logger.LogInformation("Loading basic suppliers data");
            
            var (suppliers, _) = await GetDataFromApiAsync<SupplierDto>("api/data/suppliers");

            _logger.LogInformation("Successfully loaded {SupplierCount} suppliers", suppliers.Count);
            return suppliers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load suppliers");
            return new List<SupplierDto>();
        }
    }

    /// <summary>
    /// Load orders (no additional calculations needed currently)
    /// </summary>
    public async Task<List<OrderDto>> LoadOrdersAsync()
    {
        try
        {
            _logger.LogInformation("Loading orders");
            
            var (orders, _) = await GetDataFromApiAsync<OrderDto>("api/data/orders");

            _logger.LogInformation("Successfully loaded {OrderCount} orders", orders.Count);
            return orders;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load orders");
            return new List<OrderDto>();
        }
    }
}
