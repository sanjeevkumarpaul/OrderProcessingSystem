using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Cache.Interfaces;

namespace OrderProcessingSystem.Cache.Implementation;

public class GridColumnCacheService : IGridColumnCacheService
{
    private readonly ICacheService _cache;
    private const string GRID_COLUMNS_PREFIX = "grid_columns:";
    private const string UI_GRID_COLUMNS_PREFIX = "ui_grid_columns:";
    
    // Cache for 30 minutes - grid metadata doesn't change often
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(30);

    public GridColumnCacheService(ICacheService cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<List<GridColumnDto>?> GetGridColumnsAsync(string gridName)
    {
        var key = $"{GRID_COLUMNS_PREFIX}{gridName.ToLowerInvariant()}";
        return await _cache.GetAsync<List<GridColumnDto>>(key);
    }

    public async Task SetGridColumnsAsync(string gridName, List<GridColumnDto> columns)
    {
        var key = $"{GRID_COLUMNS_PREFIX}{gridName.ToLowerInvariant()}";
        await _cache.SetAsync(key, columns, DefaultExpiry);
    }

    public async Task<List<UIGridColumnDto>?> GetUIGridColumnsAsync(string gridName)
    {
        var key = $"{UI_GRID_COLUMNS_PREFIX}{gridName.ToLowerInvariant()}";
        return await _cache.GetAsync<List<UIGridColumnDto>>(key);
    }

    public async Task SetUIGridColumnsAsync(string gridName, List<UIGridColumnDto> columns)
    {
        var key = $"{UI_GRID_COLUMNS_PREFIX}{gridName.ToLowerInvariant()}";
        await _cache.SetAsync(key, columns, DefaultExpiry);
    }

    public async Task ClearAllGridColumnsAsync()
    {
        // Note: In a real Redis implementation, you'd use SCAN with pattern matching
        // For now, we'll clear known grid types
        var gridNames = new[] { "customers", "orders", "suppliers" };
        
        var tasks = new List<Task>();
        foreach (var gridName in gridNames)
        {
            tasks.Add(ClearGridColumnsAsync(gridName));
        }
        
        await Task.WhenAll(tasks);
    }

    public async Task ClearGridColumnsAsync(string gridName)
    {
        var gridKey = $"{GRID_COLUMNS_PREFIX}{gridName.ToLowerInvariant()}";
        var uiGridKey = $"{UI_GRID_COLUMNS_PREFIX}{gridName.ToLowerInvariant()}";
        
        await Task.WhenAll(
            _cache.RemoveAsync(gridKey),
            _cache.RemoveAsync(uiGridKey)
        );
    }
}
