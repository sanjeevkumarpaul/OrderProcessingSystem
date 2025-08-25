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
            catch
            {
                // Fall through to fallback logic
            }
        }

        // Fallback to hard-coded columns if metadata loading fails
        var fallbackColumns = GetFallbackColumns(name.ToLowerInvariant());
        if (fallbackColumns != null)
        {
            return Ok(fallbackColumns);
        }

        return NotFound($"No metadata or fallback available for grid: {name}");
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
            catch
            {
                // Fall through to fallback logic
            }
        }

        // Fallback to hard-coded UI columns using AutoMapper
        var fallbackContractColumns = GetFallbackColumns(name.ToLowerInvariant());
        if (fallbackContractColumns != null)
        {
            var fallbackUIColumns = _mappingService.MapToUIColumns(fallbackContractColumns);
            return Ok(fallbackUIColumns);
        }

        return NotFound($"No UI metadata available for grid: {name}");
    }

    private static List<GridColumnDto>? GetFallbackColumns(string gridName)
    {
        return gridName switch
        {
            "customers" => new List<GridColumnDto>
            {
                new() { Header = "Name", Field = "Name", Filterable = true, Sortable = true },
                new() { Header = "Orders", Field = "OrdersCount", Sortable = true, IsNumeric = true },
                new() { Header = "Total Sales", Field = "TotalSales", Sortable = true, IsNumeric = true }
            },
            "orders" => new List<GridColumnDto>
            {
                new() { Header = "Order", Field = "OrderId", Sortable = true },
                new() { Header = "Customer", Field = "Customer.Name", Filterable = true, Sortable = true },
                new() { Header = "Supplier", Field = "Supplier.Name", Filterable = true, Sortable = true },
                new() { Header = "Total", Field = "Total", Sortable = true, IsNumeric = true },
                new() { Header = "Status", Field = "Status", Filterable = true, Sortable = true, IsEnum = true }
            },
            "suppliers" => new List<GridColumnDto>
            {
                new() { Header = "Name", Field = "Name", Filterable = true, Sortable = true },
                new() { Header = "Country", Field = "Country", Filterable = true, Sortable = true },
                new() { Header = "Orders Supplied", Field = "OrdersSupplied", Sortable = true, IsNumeric = true }
            },
            _ => null
        };
    }
}
