using OrderProcessingSystem.Infrastructure.Models;
using OrderProcessingSystem.Infrastructure.Services;
using OrderProcessingSystem.Contracts.Dto;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace OrderProcessingSystem.Infrastructure.Services;

/// <summary>
/// Implementation of grid column service for loading metadata from API endpoints
/// </summary>
public class GridColumnService : IGridColumnService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GridColumnService> _logger;

    public GridColumnService(IHttpClientFactory httpClientFactory, ILogger<GridColumnService> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<GridColumn>> LoadColumnMetadataAsync(string entityName)
    {
        return await LoadColumnMetadataAsync(entityName, null);
    }

    public async Task<List<GridColumn>> LoadColumnMetadataAsync(string entityName, Dictionary<string, List<string>>? enumMappings)
    {
        try
        {
            _logger.LogInformation("Loading column metadata for entity: {EntityName}", entityName);
            
            var client = _httpClientFactory.CreateClient("ApiClient");
            var endpoint = $"api/metadata/ui-grid-columns/{entityName.ToLowerInvariant()}";
            
            // Get the DTO from API
            var dtoColumns = await client.GetFromJsonAsync<List<UIGridColumnDto>>(endpoint)
                ?? new List<UIGridColumnDto>();

            // Map to GridColumn models
            var columns = dtoColumns.Select(dto => new GridColumn
            {
                Header = dto.Header,
                Field = dto.Field,
                Sortable = dto.Sortable,
                Filterable = dto.Filterable,
                IsNumeric = dto.IsNumeric,
                IsEnum = dto.IsEnum,
                EnumValues = dto.IsEnum && enumMappings != null && enumMappings.TryGetValue(dto.Field, out var enumValues) 
                    ? enumValues 
                    : null
            }).ToList();

            _logger.LogInformation("Successfully loaded {ColumnCount} columns for entity: {EntityName}", 
                columns.Count, entityName);
            
            return columns;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load column metadata for entity: {EntityName}", entityName);
            // Return empty list as fallback
            return new List<GridColumn>();
        }
    }
}
