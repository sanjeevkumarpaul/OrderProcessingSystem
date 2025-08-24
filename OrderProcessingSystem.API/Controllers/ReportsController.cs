using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using OrderProcessingSystem.Infrastructure.Interfaces;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IApiDataService _apiDataService;
    private readonly IMapper _mapper;

    public ReportsController(IApiDataService apiDataService, IMapper mapper) => (_apiDataService, _mapper) = (apiDataService, mapper);

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string dataset = "orders", [FromQuery] string? text = null, [FromQuery] int? customerId = null, [FromQuery] int? top = null)
    {
        if (string.Equals(dataset, "orders", StringComparison.OrdinalIgnoreCase))
        {
            var orders = await _apiDataService.GetOrdersAsync();
            var list = orders.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(text)) list = list.Where(o => (o.Customer?.Name ?? string.Empty).Contains(text, StringComparison.OrdinalIgnoreCase) || (o.Supplier?.Name ?? string.Empty).Contains(text, StringComparison.OrdinalIgnoreCase));
            if (customerId.HasValue) list = list.Where(o => o.CustomerId == customerId.Value);
            // top == 0 convention means 'no limit'
            if (top.HasValue && top.Value > 0) list = list.Take(top.Value);
            var dto = _mapper.Map<List<OrderDto>>(list.ToList());
            return Ok(dto);
        }

        if (string.Equals(dataset, "suppliers", StringComparison.OrdinalIgnoreCase))
        {
            var suppliers = await _apiDataService.GetSuppliersAsync();
            var list = suppliers.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(text)) list = list.Where(s => (s.Name ?? string.Empty).Contains(text, StringComparison.OrdinalIgnoreCase) || (s.Country ?? string.Empty).Contains(text, StringComparison.OrdinalIgnoreCase));
            var dto = _mapper.Map<List<SupplierDto>>(list.ToList());
            return Ok(dto);
        }

        return BadRequest("Unknown dataset");
    }
}
