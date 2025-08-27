using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.MediatorCQRS.Orders;

public record UpdateOrderCommand(Order Order) : IRequest;
