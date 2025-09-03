using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Contracts.Interfaces;
using OrderProcessingSystem.Core.Configuration;

namespace OrderProcessingSystem.Infrastructure.Services;

/// <summary>
/// HTTP-based implementation of IUserLogService for client applications
/// This service makes HTTP calls to the UserLog API instead of direct database access
/// Used by UI and other client applications to maintain clean architecture separation
/// </summary>
public class HttpUserLogService : IUserLogService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpUserLogService> _logger;
    private readonly ApiEndpointsConfiguration _endpoints;
    private readonly IAuthStateService _authStateService;

    public HttpUserLogService(IHttpClientFactory httpClientFactory, ILogger<HttpUserLogService> logger, IOptions<ApiEndpointsConfiguration> endpointsOptions, IAuthStateService authStateService)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
        _endpoints = endpointsOptions.Value;
        _authStateService = authStateService;
    }

    public async Task<LoginEventResponse> LogLoginEventAsync(LoginEventRequest request)
    {
        try
        {
            _logger.LogInformation("Sending login event request to API for event type: {EventType}", request.EventType);

            var response = await _httpClient.PostAsJsonAsync("/api/userlog/login-event", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginEventResponse>();
                return result ?? new LoginEventResponse { Success = false, Message = "Empty response from API" };
            }

            _logger.LogError("API call failed with status code: {StatusCode}", response.StatusCode);
            return new LoginEventResponse
            {
                Success = false,
                Message = $"API call failed with status: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserLog API for login event");
            return new LoginEventResponse
            {
                Success = false,
                Message = $"Error calling API: {ex.Message}"
            };
        }
    }

    public async Task<UserLogListResponse> GetUserLogsAsync(int pageNumber = 1, int pageSize = 50, string? eventType = null)
    {
        try
        {
            _logger.LogInformation("Requesting user logs from API - Page: {PageNumber}, Size: {PageSize}, EventType: {EventType}", 
                pageNumber, pageSize, eventType ?? "All");

            var url = $"/api/userlog?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrEmpty(eventType))
            {
                url += $"&eventType={Uri.EscapeDataString(eventType)}";
            }

            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserLogListResponse>();
                _logger.LogInformation("Successfully retrieved {Count} user logs from API", result?.UserLogs?.Count ?? 0);
                return result ?? new UserLogListResponse();
            }

            _logger.LogError("API call failed with status code: {StatusCode}", response.StatusCode);
            return new UserLogListResponse
            {
                UserLogs = new List<UserLogDto>(),
                TotalCount = 0,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserLog API for getting logs");
            return new UserLogListResponse
            {
                UserLogs = new List<UserLogDto>(),
                TotalCount = 0,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = 0
            };
        }
    }
}
