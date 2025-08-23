using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Features.Orders;

public record UpdateOrderCommand(Order Order) : IRequest;
