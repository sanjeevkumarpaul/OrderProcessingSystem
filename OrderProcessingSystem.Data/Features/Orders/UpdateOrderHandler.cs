using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Repositories;

namespace OrderProcessingSystem.Data.Features.Orders;

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
