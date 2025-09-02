using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Contracts.Interfaces;

namespace OrderProcessingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly IApiDataService _apiDataService;
    private readonly IMapper _mapper;

    public DataController(IApiDataService apiDataService, IMapper mapper) => (_apiDataService, _mapper) = (apiDataService, mapper);

    [HttpGet("orders")]
    public async Task<IActionResult> Orders()
    {
        var orders = await _apiDataService.GetOrdersAsync();
        var dto = _mapper.Map<List<OrderDto>>(orders);
        return Ok(dto);
    }

    [HttpGet("suppliers")]
    public async Task<IActionResult> Suppliers()
    {
        var suppliers = await _apiDataService.GetSuppliersAsync();
        var dto = _mapper.Map<List<SupplierDto>>(suppliers);
        return Ok(dto);
    }

    [HttpGet("customers")]
    public async Task<IActionResult> Customers()
    {
        var customers = await _apiDataService.GetCustomersAsync();
        var dto = _mapper.Map<List<CustomerDto>>(customers);
        return Ok(dto);
    }

    [HttpGet("customers-with-orders")]
    public async Task<IActionResult> CustomersWithOrders()
    {
        var customers = await _apiDataService.GetCustomersWithOrdersAsync();
        return Ok(customers); // No need to map since ApiDataService already returns the correct DTO
    }

    [HttpGet("suppliers-with-orders")]
    public async Task<IActionResult> SuppliersWithOrders()
    {
        var suppliers = await _apiDataService.GetSuppliersWithOrdersAsync();
        return Ok(suppliers); // No need to map since ApiDataService already returns the correct DTO
    }

    [HttpGet("reports/sales-by-customer")]
    public async Task<IActionResult> SalesByCustomer([FromQuery] int? customerId = null, [FromQuery] int? top = null)
    {
        var report = await _apiDataService.GetSalesByCustomerAsync(customerId, top);
        var dto = _mapper.Map<List<SalesByCustomerDto>>(report);
        return Ok(dto);
    }
}