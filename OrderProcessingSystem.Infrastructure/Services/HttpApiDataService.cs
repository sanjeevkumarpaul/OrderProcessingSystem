using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Contracts.Interfaces;
using OrderProcessingSystem.Core.Configuration;

namespace OrderProcessingSystem.Infrastructure.Services;

/// <summary>
/// HTTP-based implementation of IApiDataService for client applications
/// This service makes HTTP calls to the API instead of direct database access
/// Used by UI and other client applications to maintain clean architecture separation
/// </summary>
public class HttpApiDataService : IApiDataService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpApiDataService> _logger;
    private readonly ApiEndpointsConfiguration _endpoints;

    public HttpApiDataService(IHttpClientFactory httpClientFactory, ILogger<HttpApiDataService> logger, IOptions<ApiEndpointsConfiguration> endpointsOptions)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
        _endpoints = endpointsOptions.Value;
    }

    /// <summary>
    /// Generic method to handle all HTTP GET requests with centralized error handling
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to</typeparam>
    /// <param name="endpoint">The API endpoint to call</param>
    /// <param name="operationName">Name of the operation for logging purposes</param>
    /// <returns>List of T or empty list if error occurs</returns>
    private async Task<List<T>> GetFromApiAsync<T>(string endpoint, string operationName)
    {
        try
        {
            _logger.LogInformation("Fetching {Operation} from API: {Endpoint}", operationName, endpoint);
            var response = await _httpClient.GetFromJsonAsync<List<T>>(endpoint);
            _logger.LogDebug("Successfully fetched {Count} {Operation} records", response?.Count ?? 0, operationName);
            return response ?? new List<T>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching {Operation} from API endpoint {Endpoint}: {StatusCode}", 
                operationName, endpoint, ex.Data.Contains("StatusCode") ? ex.Data["StatusCode"] : "Unknown");
            return new List<T>();
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Timeout error fetching {Operation} from API endpoint {Endpoint}", operationName, endpoint);
            return new List<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching {Operation} from API endpoint {Endpoint}", operationName, endpoint);
            return new List<T>();
        }
    }

    /// <summary>
    /// Generic method to handle HTTP GET requests with query parameters and centralized error handling
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to</typeparam>
    /// <param name="baseEndpoint">The base API endpoint</param>
    /// <param name="queryParams">Dictionary of query parameters</param>
    /// <param name="operationName">Name of the operation for logging purposes</param>
    /// <returns>List of T or empty list if error occurs</returns>
    private async Task<List<T>> GetFromApiWithQueryAsync<T>(string baseEndpoint, Dictionary<string, string> queryParams, string operationName)
    {
        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams.Select(kv => $"{kv.Key}={kv.Value}")) : "";
        var fullEndpoint = $"{baseEndpoint}{queryString}";
        return await GetFromApiAsync<T>(fullEndpoint, operationName);
    }

    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        return await GetFromApiAsync<OrderDto>(_endpoints.Data.Orders, "orders");
    }

    public async Task<List<SupplierDto>> GetSuppliersAsync()
    {
        return await GetFromApiAsync<SupplierDto>(_endpoints.Data.Suppliers, "suppliers");
    }

    public async Task<List<CustomerDto>> GetCustomersAsync()
    {
        return await GetFromApiAsync<CustomerDto>(_endpoints.Data.Customers, "customers");
    }

    public async Task<List<SalesByCustomerDto>> GetSalesByCustomerAsync(int? customerId = null, int? top = null)
    {
        var queryParams = new Dictionary<string, string>();
        if (customerId.HasValue)
            queryParams.Add("customerId", customerId.ToString()!);
        if (top.HasValue)
            queryParams.Add("top", top.ToString()!);

        return await GetFromApiWithQueryAsync<SalesByCustomerDto>(_endpoints.Reports.SalesByCustomer, queryParams, "sales by customer");
    }

    public async Task<List<CustomerWithOrdersDto>> GetCustomersWithOrdersAsync()
    {
        return await GetFromApiAsync<CustomerWithOrdersDto>(_endpoints.Data.CustomersWithOrders, "customers with orders");
    }

    public async Task<List<SupplierWithOrdersDto>> GetSuppliersWithOrdersAsync()
    {
        return await GetFromApiAsync<SupplierWithOrdersDto>(_endpoints.Data.SuppliersWithOrders, "suppliers with orders");
    }

    public async Task<List<TransExceptionDto>> GetTransExceptionsAsync()
    {
        return await GetFromApiAsync<TransExceptionDto>(_endpoints.TransExceptions.Base, "transaction exceptions");
    }

    public async Task<List<TransExceptionDto>> GetTransExceptionsByTypeAsync(string transactionType)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["type"] = transactionType
        };

        return await GetFromApiWithQueryAsync<TransExceptionDto>(_endpoints.TransExceptions.Base, queryParams, "transaction exceptions by type");
    }
}
