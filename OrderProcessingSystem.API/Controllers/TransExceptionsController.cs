using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Data.MediatorCQRSFeature.TransExceptions;

namespace OrderProcessingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransExceptionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public TransExceptionsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all transaction exceptions
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TransExceptionDto>>> GetAllTransExceptions()
    {
        var query = new GetAllTransExceptionsQuery();
        var result = await _mediator.Send(query);
        var dto = _mapper.Map<List<TransExceptionDto>>(result);
        return Ok(dto);
    }

    /// <summary>
    /// Get transaction exceptions by type (ORDERCREATION or ORDERCANCELLATION)
    /// </summary>
    [HttpGet("by-type/{transactionType}")]
    public async Task<ActionResult<List<TransExceptionDto>>> GetTransExceptionsByType(string transactionType)
    {
        var query = new GetTransExceptionsByTypeQuery(transactionType);
        var result = await _mediator.Send(query);
        var dto = _mapper.Map<List<TransExceptionDto>>(result);
        return Ok(dto);
    }

    /// <summary>
    /// Create a new transaction exception record
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<int>> CreateTransException([FromBody] CreateTransExceptionRequest request)
    {
        var command = new CreateTransExceptionCommand(
            request.TransactionType,
            request.InputMessage,
            request.Reason
        );
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

public class CreateTransExceptionRequest
{
    public string TransactionType { get; set; } = string.Empty;
    public string InputMessage { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
