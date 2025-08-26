using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.API.Services;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        private readonly IGridMetadataService _gridMetadataService;

        public MetadataController(IGridMetadataService gridMetadataService)
        {
            _gridMetadataService = gridMetadataService ?? throw new ArgumentNullException(nameof(gridMetadataService));
        }

        /// <summary>
        /// Get UI grid columns for a specific entity
        /// </summary>
        /// <param name="entityName">The entity name (customers, suppliers, orders)</param>
        /// <returns>List of UI grid column configurations</returns>
        [HttpGet("ui-grid-columns/{entityName}")]
        [ProducesResponseType(typeof(List<UIGridColumnDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<UIGridColumnDto>>> GetUIGridColumns(string entityName)
        {
            try
            {
                var columns = await _gridMetadataService.GetUIGridColumnsAsync(entityName);
                
                if (columns == null || columns.Count == 0)
                {
                    return NotFound($"No grid columns found for entity: {entityName}");
                }

                return Ok(columns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving grid columns: {ex.Message}");
            }
        }

        /// <summary>
        /// Clear cache for specific entity grid columns
        /// </summary>
        /// <param name="entityName">The entity name to clear cache for</param>
        /// <returns>Success message</returns>
        [HttpDelete("cache/grid-columns/{entityName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ClearGridCache(string entityName)
        {
            try
            {
                await _gridMetadataService.ClearGridCacheAsync(entityName);
                return Ok($"Cache cleared for entity: {entityName}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error clearing cache: {ex.Message}");
            }
        }
    }
}
