using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.API.Services;

/// <summary>
/// Service interface for managing grid metadata operations
/// </summary>
public interface IGridMetadataService
{
    /// <summary>
    /// Get grid columns for a specific grid with caching
    /// </summary>
    Task<List<GridColumnDto>?> GetGridColumnsAsync(string gridName);
    
    /// <summary>
    /// Get UI-optimized grid columns for a specific grid with caching
    /// </summary>
    Task<List<UIGridColumnDto>?> GetUIGridColumnsAsync(string gridName);
    
    /// <summary>
    /// Clear cache for a specific grid
    /// </summary>
    Task ClearGridCacheAsync(string gridName);
    
    /// <summary>
    /// Clear all grid column caches
    /// </summary>
    Task ClearAllGridCachesAsync();
}
