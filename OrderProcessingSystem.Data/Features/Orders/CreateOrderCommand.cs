using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Features.Orders;

public record CreateOrderCommand(Order Order) : IRequest<Order>;
