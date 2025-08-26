using System.Collections.Concurrent;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Cache;

/// <summary>
/// In-memory implementation of grid column cache service for when Redis is not available
/// </summary>
public class InMemoryGridColumnCacheService : IGridColumnCacheService
{
    private readonly ConcurrentDictionary<string, (List<GridColumnDto> data, DateTime expiry)> _gridCache = new();
    private readonly ConcurrentDictionary<string, (List<UIGridColumnDto> data, DateTime expiry)> _uiGridCache = new();
    
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(30);

    public async Task<List<GridColumnDto>?> GetGridColumnsAsync(string gridName)
    {
        var key = gridName.ToLowerInvariant();
        if (_gridCache.TryGetValue(key, out var cached))
        {
            if (DateTime.UtcNow < cached.expiry)
            {
                return cached.data;
            }
            
            // Remove expired entry
            _gridCache.TryRemove(key, out _);
        }
        
        return await Task.FromResult<List<GridColumnDto>?>(null);
    }

    public async Task SetGridColumnsAsync(string gridName, List<GridColumnDto> columns)
    {
        var key = gridName.ToLowerInvariant();
        var expiry = DateTime.UtcNow.Add(DefaultExpiry);
        _gridCache.AddOrUpdate(key, (columns, expiry), (_, _) => (columns, expiry));
        await Task.CompletedTask;
    }

    public async Task<List<UIGridColumnDto>?> GetUIGridColumnsAsync(string gridName)
    {
        var key = gridName.ToLowerInvariant();
        if (_uiGridCache.TryGetValue(key, out var cached))
        {
            if (DateTime.UtcNow < cached.expiry)
            {
                return cached.data;
            }
            
            // Remove expired entry
            _uiGridCache.TryRemove(key, out _);
        }
        
        return await Task.FromResult<List<UIGridColumnDto>?>(null);
    }

    public async Task SetUIGridColumnsAsync(string gridName, List<UIGridColumnDto> columns)
    {
        var key = gridName.ToLowerInvariant();
        var expiry = DateTime.UtcNow.Add(DefaultExpiry);
        _uiGridCache.AddOrUpdate(key, (columns, expiry), (_, _) => (columns, expiry));
        await Task.CompletedTask;
    }

    public async Task ClearAllGridColumnsAsync()
    {
        _gridCache.Clear();
        _uiGridCache.Clear();
        await Task.CompletedTask;
    }

    public async Task ClearGridColumnsAsync(string gridName)
    {
        var key = gridName.ToLowerInvariant();
        _gridCache.TryRemove(key, out _);
        _uiGridCache.TryRemove(key, out _);
        await Task.CompletedTask;
    }
}
