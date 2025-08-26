using OrderProcessingServer.Shared.Dto;
using System.Net.Http.Json;

namespace OrderProcessingServer.Services;

/// <summary>
/// Service for loading and processing data with common calculations
/// Eliminates duplication across pages for data loading logic
/// </summary>
public class DataLoadingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DataLoadingService> _logger;

    public DataLoadingService(IHttpClientFactory httpClientFactory, ILogger<DataLoadingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Load customers with calculated OrdersCount and TotalSales
    /// </summary>
    public async Task<List<CustomerDto>> LoadCustomersWithCalculationsAsync()
    {
        try
        {
            _logger.LogInformation("Loading customers with order calculations");
            
            var client = _httpClientFactory.CreateClient("ApiClient");
            
            // Load base data in parallel for better performance
            var customersTask = client.GetFromJsonAsync<List<CustomerDto>>("api/data/customers");
            var ordersTask = client.GetFromJsonAsync<List<OrderDto>>("api/data/orders");
            
            await Task.WhenAll(customersTask, ordersTask);
            
            var customers = await customersTask ?? new List<CustomerDto>();
            var orders = await ordersTask ?? new List<OrderDto>();

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
            
            var client = _httpClientFactory.CreateClient("ApiClient");
            
            // Load base data in parallel for better performance
            var suppliersTask = client.GetFromJsonAsync<List<SupplierDto>>("api/data/suppliers");
            var ordersTask = client.GetFromJsonAsync<List<OrderDto>>("api/data/orders");
            
            await Task.WhenAll(suppliersTask, ordersTask);
            
            var suppliers = await suppliersTask ?? new List<SupplierDto>();
            var orders = await ordersTask ?? new List<OrderDto>();

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
    /// Load orders (no additional calculations needed currently)
    /// </summary>
    public async Task<List<OrderDto>> LoadOrdersAsync()
    {
        try
        {
            _logger.LogInformation("Loading orders");
            
            var client = _httpClientFactory.CreateClient("ApiClient");
            var orders = await client.GetFromJsonAsync<List<OrderDto>>("api/data/orders") ?? new List<OrderDto>();

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
