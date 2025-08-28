using OrderProcessingServer.Shared.Dto;
using System.Text.Json;

namespace OrderProcessingServer;

/// <summary>
/// Service for loading and processing data with calculations from API
/// </summary>
public class DataLoadingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DataLoadingService> _logger;

    public DataLoadingService(IHttpClientFactory httpClientFactory, ILogger<DataLoadingService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    public async Task<List<CustomerWithOrdersVM>> LoadCustomersWithOrdersAsync()
    {
        try
        {
            _logger.LogInformation("Loading customers with orders from API...");
            
            var customers = await _httpClient.GetFromJsonAsync<List<CustomerWithOrdersVM>>("api/data/customers-with-orders");
            
            if (customers == null)
            {
                _logger.LogWarning("No customers data received from API");
                return new List<CustomerWithOrdersVM>();
            }

            _logger.LogInformation($"Successfully loaded {customers.Count} customers with orders from API");
            return customers;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while loading customers with orders from API");
            return new List<CustomerWithOrdersVM>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error while loading customers with orders from API");
            return new List<CustomerWithOrdersVM>();
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request to load customers with orders was cancelled");
            return new List<CustomerWithOrdersVM>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading customers with orders from API");
            return new List<CustomerWithOrdersVM>();
        }
    }

    public async Task<List<SupplierWithOrdersVM>> LoadSuppliersWithOrdersAsync()
    {
        try
        {
            _logger.LogInformation("Loading suppliers with orders from API...");
            
            var suppliers = await _httpClient.GetFromJsonAsync<List<SupplierWithOrdersVM>>("api/data/suppliers-with-orders");
            
            if (suppliers == null)
            {
                _logger.LogWarning("No suppliers data received from API");
                return new List<SupplierWithOrdersVM>();
            }

            _logger.LogInformation($"Successfully loaded {suppliers.Count} suppliers with orders from API");
            return suppliers;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while loading suppliers with orders from API");
            return new List<SupplierWithOrdersVM>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error while loading suppliers with orders from API");
            return new List<SupplierWithOrdersVM>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading suppliers with orders from API");
            return new List<SupplierWithOrdersVM>();
        }
    }

    public async Task<List<OrderVM>> LoadOrdersAsync()
    {
        try
        {
            _logger.LogInformation("Loading orders from API...");
            
            var orders = await _httpClient.GetFromJsonAsync<List<OrderVM>>("api/data/orders");
            
            if (orders == null)
            {
                _logger.LogWarning("No orders data received from API");
                return new List<OrderVM>();
            }

            _logger.LogInformation($"Successfully loaded {orders.Count} orders from API");
            return orders;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while loading orders from API");
            return new List<OrderVM>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error while loading orders from API");
            return new List<OrderVM>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading orders from API");
            return new List<OrderVM>();
        }
    }

    public async Task<List<SalesByCustomerVM>> LoadSalesByCustomerAsync()
    {
        try
        {
            _logger.LogInformation("Loading sales by customer from API...");
            
            var salesData = await _httpClient.GetFromJsonAsync<List<SalesByCustomerVM>>("api/data/sales-by-customer");
            
            if (salesData == null)
            {
                _logger.LogWarning("No sales by customer data received from API");
                return new List<SalesByCustomerVM>();
            }

            _logger.LogInformation($"Successfully loaded {salesData.Count} sales by customer records from API");
            return salesData;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while loading sales by customer from API");
            return new List<SalesByCustomerVM>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error while loading sales by customer from API");
            return new List<SalesByCustomerVM>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading sales by customer from API");
            return new List<SalesByCustomerVM>();
        }
    }
}
