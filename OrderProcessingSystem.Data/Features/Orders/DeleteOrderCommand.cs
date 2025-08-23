using MediatR;

namespace OrderProcessingSystem.Data.Features.Orders;

public record DeleteOrderCommand(int Id) : IRequest;
