using OrderProcessingServer.Shared.Dto;
using System.Text.Json;

namespace OrderProcessingServer.Services;

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

    /// <summary>
    /// Generic method to handle API calls with consistent error handling
    /// </summary>
    private async Task<List<T>> LoadDataAsync<T>(string endpoint, string dataTypeName) where T : class
    {
        try
        {
            _logger.LogInformation($"Loading {dataTypeName} from API...");
            
            var data = await _httpClient.GetFromJsonAsync<List<T>>(endpoint);
            
            if (data == null)
            {
                _logger.LogWarning($"No {dataTypeName} data received from API");
                return new List<T>();
            }

            _logger.LogInformation($"Successfully loaded {data.Count} {dataTypeName} from API");
            return data;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP error while loading {dataTypeName} from API");
            return new List<T>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, $"JSON deserialization error while loading {dataTypeName} from API");
            return new List<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while loading {dataTypeName} from API");
            return new List<T>();
        }
    }

    public async Task<List<CustomerWithOrdersDto>> LoadCustomersWithOrdersAsync()
    {
        return await LoadDataAsync<CustomerWithOrdersDto>("api/data/customers-with-orders", "customers with orders");
    }

    public async Task<List<SupplierWithOrdersDto>> LoadSuppliersWithOrdersAsync()
    {
        return await LoadDataAsync<SupplierWithOrdersDto>("api/data/suppliers-with-orders", "suppliers with orders");
    }

    public async Task<List<OrderDto>> LoadOrdersAsync()
    {
        return await LoadDataAsync<OrderDto>("api/data/orders", "orders");
    }

    public async Task<List<SalesByCustomerDto>> LoadSalesByCustomerAsync()
    {
        return await LoadDataAsync<SalesByCustomerDto>("api/data/sales-by-customer", "sales by customer");
    }
}
