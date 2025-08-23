using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Repositories;

namespace OrderProcessingSystem.Data.Features.Orders;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly IOrderRepository _repo;
    public CreateOrderHandler(IOrderRepository repo) => _repo = repo;

    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        await _repo.AddAsync(request.Order, cancellationToken);
        return request.Order;
    }
}
