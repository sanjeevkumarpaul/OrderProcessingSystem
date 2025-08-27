using MediatR;

namespace OrderProcessingSystem.Data.MediatorCQRS.Orders;

public record DeleteOrderCommand(int Id) : IRequest;
