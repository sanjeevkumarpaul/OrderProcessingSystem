using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.MediatorCQRS.Orders;

public record CreateOrderCommand(Order Order) : IRequest<Order>;
