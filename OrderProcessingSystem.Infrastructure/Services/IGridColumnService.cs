using OrderProcessingSystem.Infrastructure.Models;

namespace OrderProcessingSystem.Infrastructure.Services;

/// <summary>
/// Service for loading grid column metadata from API endpoints
/// </summary>
public interface IGridColumnService
{
    /// <summary>
    /// Load grid column metadata for a specific entity
    /// </summary>
    /// <param name="entityName">Entity name (customers, suppliers, orders)</param>
    /// <returns>List of grid columns for the entity</returns>
    Task<List<GridColumn>> LoadColumnMetadataAsync(string entityName);
    
    /// <summary>
    /// Load grid column metadata for a specific entity with enum values mapping
    /// </summary>
    /// <param name="entityName">Entity name (customers, suppliers, orders)</param>
    /// <param name="enumMappings">Dictionary mapping field names to their enum values</param>
    /// <returns>List of grid columns for the entity with enum values populated</returns>
    Task<List<GridColumn>> LoadColumnMetadataAsync(string entityName, Dictionary<string, List<string>>? enumMappings);
}
