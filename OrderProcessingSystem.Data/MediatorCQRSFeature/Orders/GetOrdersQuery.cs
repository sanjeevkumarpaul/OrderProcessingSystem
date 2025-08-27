using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.MediatorCQRS.Orders;

public record GetOrdersQuery : IRequest<List<Order>>;
