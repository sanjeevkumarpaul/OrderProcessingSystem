using Microsoft.AspNetCore.Mvc;
using MediatR;
using OrderProcessingSystem.Data.MediatorCQRSFeature.Orders;
using OrderProcessingSystem.Events.Models;
using System.Text.Json;

namespace OrderProcessingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderProcessingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderProcessingController> _logger;

    public OrderProcessingController(IMediator mediator, ILogger<OrderProcessingController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("process-order-transaction")]
    public async Task<IActionResult> ProcessOrderTransaction([FromBody] OrderTransactionSchema orderTransaction)
    {
        try
        {
            _logger.LogInformation("Received order transaction for processing: Customer={Customer}, Supplier={Supplier}, Quantity={Quantity}, Price={Price}",
                orderTransaction.Customer.Name, orderTransaction.Supplier.Name, 
                orderTransaction.Supplier.Quantity, orderTransaction.Supplier.Price);

            var command = new ProcessOrderTransactionCommand(orderTransaction);
            var result = await _mediator.Send(command);

            _logger.LogInformation("Successfully processed order transaction. Created Order ID: {OrderId}", result.OrderId);

            return Ok(new
            {
                OrderId = result.OrderId,
                CustomerId = result.CustomerId,
                SupplierId = result.SupplierId,
                Total = result.Total,
                Status = result.Status,
                Message = "Order transaction processed successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order transaction for Customer={Customer}, Supplier={Supplier}",
                orderTransaction.Customer.Name, orderTransaction.Supplier.Name);
            
            return StatusCode(500, new
            {
                Message = "Failed to process order transaction",
                Error = ex.Message
            });
        }
    }

    [HttpPost("process-order-cancellation")]
    public async Task<IActionResult> ProcessOrderCancellation([FromBody] OrderCancellationSchema orderCancellation)
    {
        try
        {
            _logger.LogInformation("Received order cancellation for processing: Customer={Customer}, Supplier={Supplier}, Quantity={Quantity}",
                orderCancellation.Customer, orderCancellation.Supplier, orderCancellation.Quantity);

            var command = new ProcessOrderCancellationCommand(orderCancellation);
            var result = await _mediator.Send(command);

            if (result)
            {
                _logger.LogInformation("Successfully processed order cancellation for Customer={Customer}, Supplier={Supplier}",
                    orderCancellation.Customer, orderCancellation.Supplier);

                return Ok(new
                {
                    Message = "Order cancellation processed successfully",
                    Customer = orderCancellation.Customer,
                    Supplier = orderCancellation.Supplier,
                    ProcessedSuccessfully = true
                });
            }
            else
            {
                _logger.LogWarning("No orders found to cancel for Customer={Customer}, Supplier={Supplier}",
                    orderCancellation.Customer, orderCancellation.Supplier);

                return NotFound(new
                {
                    Message = "No orders found to cancel for the specified Customer and Supplier",
                    Customer = orderCancellation.Customer,
                    Supplier = orderCancellation.Supplier,
                    ProcessedSuccessfully = false
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order cancellation for Customer={Customer}, Supplier={Supplier}",
                orderCancellation.Customer, orderCancellation.Supplier);
            
            return StatusCode(500, new
            {
                Message = "Failed to process order cancellation",
                Error = ex.Message
            });
        }
    }
}
