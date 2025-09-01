using OrderProcessingServer.Shared.UIViewModels;
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

    public async Task<List<CustomerWithOrdersVM>> LoadCustomersWithOrdersAsync()
    {
        return await LoadDataAsync<CustomerWithOrdersVM>("api/data/customers-with-orders", "customers with orders");
    }

    public async Task<List<SupplierWithOrdersVM>> LoadSuppliersWithOrdersAsync()
    {
        return await LoadDataAsync<SupplierWithOrdersVM>("api/data/suppliers-with-orders", "suppliers with orders");
    }

    public async Task<List<CustomerVM>> LoadCustomersAsync()
    {
        return await LoadDataAsync<CustomerVM>("api/data/customers", "customers");
    }

    public async Task<List<SupplierVM>> LoadSuppliersAsync()
    {
        return await LoadDataAsync<SupplierVM>("api/data/suppliers", "suppliers");
    }

    public async Task<List<OrderVM>> LoadOrdersAsync()
    {
        return await LoadDataAsync<OrderVM>("api/data/orders", "orders");
    }

    public async Task<List<SalesByCustomerVM>> LoadSalesByCustomerAsync()
    {
        return await LoadDataAsync<SalesByCustomerVM>("api/data/sales-by-customer", "sales by customer");
    }

    public async Task<List<TransExceptionVM>> LoadTransExceptionsAsync()
    {
        return await LoadDataAsync<TransExceptionVM>("api/TransExceptions", "transaction exceptions");
    }

    public async Task<List<TransExceptionVM>> LoadTransExceptionsByTypeAsync(string transactionType)
    {
        return await LoadDataAsync<TransExceptionVM>($"api/TransExceptions/by-type/{transactionType}", $"transaction exceptions for {transactionType}");
    }
}
