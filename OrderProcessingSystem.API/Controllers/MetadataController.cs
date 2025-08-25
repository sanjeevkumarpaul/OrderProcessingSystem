using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using OrderProcessingSystem.Core.Metadata;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.API.Controllers;

// Simple UI GridColumn DTO to avoid client-side mapping
public class UIGridColumnDto
{
    public string Header { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public bool Sortable { get; set; }
    public bool Filterable { get; set; }
    public bool IsNumeric { get; set; }
    public bool IsEnum { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class MetadataController : ControllerBase
{
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
                // Convert directly to UI format at server level
                var dtoList = JsonSerializer.Deserialize<List<GridColumnDto>>(arr.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (dtoList != null && dtoList.Count > 0)
                {
                    var uiColumns = dtoList.Select(dto => new UIGridColumnDto
                    {
                        Header = dto.Header,
                        Field = dto.Field,
                        Sortable = dto.Sortable,
                        Filterable = dto.Filterable,
                        IsNumeric = dto.IsNumeric,
                        IsEnum = dto.IsEnum
                    }).ToList();
                    return Ok(uiColumns);
                }
            }
            catch
            {
                // Fall through to fallback logic
            }
        }

        // Fallback to hard-coded UI columns
        var fallbackUIColumns = GetFallbackUIColumns(name.ToLowerInvariant());
        if (fallbackUIColumns != null)
        {
            return Ok(fallbackUIColumns);
        }

        return NotFound($"No UI metadata available for grid: {name}");
    }

    private static List<UIGridColumnDto>? GetFallbackUIColumns(string gridName)
    {
        return gridName switch
        {
            "customers" => new List<UIGridColumnDto>
            {
                new() { Header = "Name", Field = "Name", Filterable = true, Sortable = true },
                new() { Header = "Orders", Field = "OrdersCount", Sortable = true, IsNumeric = true },
                new() { Header = "Total Sales", Field = "TotalSales", Sortable = true, IsNumeric = true }
            },
            "suppliers" => new List<UIGridColumnDto>
            {
                new() { Header = "Name", Field = "Name", Sortable = true, Filterable = true },
                new() { Header = "Country", Field = "Country", Sortable = true, Filterable = true },
                new() { Header = "Orders Supplied", Field = "OrdersSupplied", Sortable = true, IsNumeric = true }
            },
            "orders" => new List<UIGridColumnDto>
            {
                new() { Header = "Order", Field = "OrderId", Sortable = true },
                new() { Header = "Customer", Field = "Customer.Name", Sortable = true, Filterable = true },
                new() { Header = "Total", Field = "Total", Sortable = true, IsNumeric = true },
                new() { Header = "Status", Field = "Status", Sortable = true, Filterable = true, IsEnum = true }
            },
            _ => null
        };
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
