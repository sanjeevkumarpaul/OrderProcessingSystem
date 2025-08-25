using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using OrderProcessingSystem.Core.Metadata;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetadataController : ControllerBase
{
    [HttpGet("grid-columns/{name}")]
    public IActionResult GetGridColumns(string name)
    {
        var doc = GridMetadataProvider.ReadMetadata();
        if (doc == null) return NotFound();
        if (!doc.RootElement.TryGetProperty(name, out var arr)) return NotFound();
        try
        {
            var list = JsonSerializer.Deserialize<List<GridColumnDto>>(arr.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Ok(list);
        }
        catch
        {
            return StatusCode(500);
        }
    }
}
