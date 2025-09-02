using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Contracts.Interfaces;

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

    public HttpApiDataService(IHttpClientFactory httpClientFactory, ILogger<HttpApiDataService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        try
        {
            _logger.LogInformation("Fetching orders from API");
            var response = await _httpClient.GetFromJsonAsync<List<OrderDto>>("api/data/orders");
            return response ?? new List<OrderDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching orders from API");
            return new List<OrderDto>();
        }
    }

    public async Task<List<SupplierDto>> GetSuppliersAsync()
    {
        try
        {
            _logger.LogInformation("Fetching suppliers from API");
            var response = await _httpClient.GetFromJsonAsync<List<SupplierDto>>("api/data/suppliers");
            return response ?? new List<SupplierDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching suppliers from API");
            return new List<SupplierDto>();
        }
    }

    public async Task<List<CustomerDto>> GetCustomersAsync()
    {
        try
        {
            _logger.LogInformation("Fetching customers from API");
            var response = await _httpClient.GetFromJsonAsync<List<CustomerDto>>("api/data/customers");
            return response ?? new List<CustomerDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching customers from API");
            return new List<CustomerDto>();
        }
    }

    public async Task<List<SalesByCustomerDto>> GetSalesByCustomerAsync(int? customerId = null, int? top = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (customerId.HasValue)
                queryParams.Add($"customerId={customerId}");
            if (top.HasValue)
                queryParams.Add($"top={top}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var url = $"api/data/reports/sales-by-customer{queryString}";

            _logger.LogInformation("Fetching sales by customer from API: {Url}", url);
            var response = await _httpClient.GetFromJsonAsync<List<SalesByCustomerDto>>(url);
            return response ?? new List<SalesByCustomerDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sales by customer from API");
            return new List<SalesByCustomerDto>();
        }
    }

    public async Task<List<CustomerWithOrdersDto>> GetCustomersWithOrdersAsync()
    {
        try
        {
            _logger.LogInformation("Fetching customers with orders from API");
            var response = await _httpClient.GetFromJsonAsync<List<CustomerWithOrdersDto>>("api/data/customers-with-orders");
            return response ?? new List<CustomerWithOrdersDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching customers with orders from API");
            return new List<CustomerWithOrdersDto>();
        }
    }

    public async Task<List<SupplierWithOrdersDto>> GetSuppliersWithOrdersAsync()
    {
        try
        {
            _logger.LogInformation("Fetching suppliers with orders from API");
            var response = await _httpClient.GetFromJsonAsync<List<SupplierWithOrdersDto>>("api/data/suppliers-with-orders");
            return response ?? new List<SupplierWithOrdersDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching suppliers with orders from API");
            return new List<SupplierWithOrdersDto>();
        }
    }

    public async Task<List<TransExceptionDto>> GetTransExceptionsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching transaction exceptions from API");
            var response = await _httpClient.GetFromJsonAsync<List<TransExceptionDto>>("api/transexceptions");
            return response ?? new List<TransExceptionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching transaction exceptions from API");
            return new List<TransExceptionDto>();
        }
    }

    public async Task<List<TransExceptionDto>> GetTransExceptionsByTypeAsync(string transactionType)
    {
        try
        {
            _logger.LogInformation("Fetching transaction exceptions by type from API: {Type}", transactionType);
            var response = await _httpClient.GetFromJsonAsync<List<TransExceptionDto>>($"api/transexceptions?type={transactionType}");
            return response ?? new List<TransExceptionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching transaction exceptions by type from API");
            return new List<TransExceptionDto>();
        }
    }
}
