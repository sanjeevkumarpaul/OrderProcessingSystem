using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Repositories;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRS.Orders;

public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly IOrderRepository _repo;
    public UpdateOrderHandler(IOrderRepository repo) => _repo = repo;

    public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        await _repo.UpdateAsync(request.Order, cancellationToken);
        return Unit.Value;
    }
}
