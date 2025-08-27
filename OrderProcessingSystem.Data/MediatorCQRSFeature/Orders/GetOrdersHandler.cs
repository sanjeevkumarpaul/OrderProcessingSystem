using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Repositories;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRS.Orders;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, List<Order>>
{
    private readonly IOrderRepository _repo;
    public GetOrdersHandler(IOrderRepository repo) => _repo = repo;

    public async Task<List<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetAllAsync(cancellationToken);
    }
}
