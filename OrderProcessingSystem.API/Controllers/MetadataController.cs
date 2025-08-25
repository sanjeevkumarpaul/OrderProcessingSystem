using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using OrderProcessingSystem.Core.Metadata;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.API.Models;
using OrderProcessingSystem.API.Services;

namespace OrderProcessingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetadataController : ControllerBase
{
    private readonly IGridColumnMappingService _mappingService;

    public MetadataController(IGridColumnMappingService mappingService)
    {
        _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
    }
    [HttpGet("grid-columns/{name}")]
    public IActionResult GetGridColumns(string name)
    {
        var doc = GridMetadataProvider.ReadMetadata();
        if (doc != null && doc.RootElement.TryGetProperty(name, out var arr))
        {
            try
            {
                var list = JsonSerializer.Deserialize<List<GridColumnDto>>(arr.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (list != null && list.Count > 0)
                {
                    return Ok(list);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deserializing metadata for grid '{name}': {ex.Message}");
            }
        }

        return NotFound($"No metadata found for grid: {name}. Please ensure the grid name exists in the metadata JSON file.");
    }

    // NEW: Direct UI-ready endpoint - no client-side mapping needed!
    [HttpGet("ui-grid-columns/{name}")]
    public IActionResult GetUIGridColumns(string name)
    {
        var doc = GridMetadataProvider.ReadMetadata();
        if (doc != null && doc.RootElement.TryGetProperty(name, out var arr))
        {
            try
            {
                // Use AutoMapper for clean, maintainable DTO transformation
                var dtoList = JsonSerializer.Deserialize<List<GridColumnDto>>(arr.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (dtoList != null && dtoList.Count > 0)
                {
                    var uiColumns = _mappingService.MapToUIColumns(dtoList);
                    return Ok(uiColumns);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing UI metadata for grid '{name}': {ex.Message}");
            }
        }

        return NotFound($"No UI metadata found for grid: {name}. Please ensure the grid name exists in the metadata JSON file.");
    }

}
