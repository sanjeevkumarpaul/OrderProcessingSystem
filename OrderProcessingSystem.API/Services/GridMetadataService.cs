using System.Text.Json;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Core.Metadata;
using OrderProcessingSystem.Cache;

namespace OrderProcessingSystem.API.Services;

/// <summary>
/// Service for managing grid metadata operations with caching and fallback logic
/// </summary>
public class GridMetadataService : IGridMetadataService
{
    private readonly IGridColumnCacheService _cacheService;
    private readonly IGridColumnMappingService _mappingService;

    public GridMetadataService(
        IGridColumnCacheService cacheService,
        IGridColumnMappingService mappingService)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
    }

    public async Task<List<GridColumnDto>?> GetGridColumnsAsync(string gridName)
    {
        // Try cache first
        var cachedColumns = await _cacheService.GetGridColumnsAsync(gridName);
        if (cachedColumns != null)
        {
            return cachedColumns;
        }

        // Cache miss - load from metadata
        var columns = LoadGridColumnsFromMetadata(gridName);
        if (columns != null)
        {
            // Cache the result for next time
            await _cacheService.SetGridColumnsAsync(gridName, columns);
        }

        return columns;
    }

    public async Task<List<UIGridColumnDto>?> GetUIGridColumnsAsync(string gridName)
    {
        // Try cache first for UI-optimized columns
        var cachedUIColumns = await _cacheService.GetUIGridColumnsAsync(gridName);
        if (cachedUIColumns != null)
        {
            return cachedUIColumns;
        }

        // Cache miss - load from metadata and transform
        var columns = LoadGridColumnsFromMetadata(gridName);
        if (columns != null)
        {
            var uiColumns = _mappingService.MapToUIColumns(columns);
            
            // Cache both the original DTOs and UI-ready columns
            await _cacheService.SetGridColumnsAsync(gridName, columns);
            await _cacheService.SetUIGridColumnsAsync(gridName, uiColumns);
            
            return uiColumns;
        }

        return null;
    }

    public async Task ClearGridCacheAsync(string gridName)
    {
        await _cacheService.ClearGridColumnsAsync(gridName);
    }

    public async Task ClearAllGridCachesAsync()
    {
        await _cacheService.ClearAllGridColumnsAsync();
    }

    /// <summary>
    /// Load grid columns from JSON metadata file
    /// </summary>
    private static List<GridColumnDto>? LoadGridColumnsFromMetadata(string gridName)
    {
        try
        {
            var doc = GridMetadataProvider.ReadMetadata();
            if (doc != null && doc.RootElement.TryGetProperty(gridName, out var arr))
            {
                var columns = JsonSerializer.Deserialize<List<GridColumnDto>>(
                    arr.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                return columns?.Count > 0 ? columns : null;
            }
        }
        catch (Exception)
        {
            // Log the exception in a real application
            // For now, return null to indicate failure
        }

        return null;
    }
}
