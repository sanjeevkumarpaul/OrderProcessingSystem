using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Cache;

/// <summary>
/// Specialized cache service for grid column metadata
/// </summary>
public interface IGridColumnCacheService
{
    /// <summary>
    /// Get cached grid columns for a specific grid
    /// </summary>
    Task<List<GridColumnDto>?> GetGridColumnsAsync(string gridName);
    
    /// <summary>
    /// Cache grid columns for a specific grid
    /// </summary>
    Task SetGridColumnsAsync(string gridName, List<GridColumnDto> columns);
    
    /// <summary>
    /// Get cached UI-ready grid columns for a specific grid
    /// </summary>
    Task<List<UIGridColumnDto>?> GetUIGridColumnsAsync(string gridName);
    
    /// <summary>
    /// Cache UI-ready grid columns for a specific grid
    /// </summary>
    Task SetUIGridColumnsAsync(string gridName, List<UIGridColumnDto> columns);
    
    /// <summary>
    /// Clear all grid column cache entries
    /// </summary>
    Task ClearAllGridColumnsAsync();
    
    /// <summary>
    /// Clear cache for a specific grid
    /// </summary>
    Task ClearGridColumnsAsync(string gridName);
}
