using MediatR;
using OrderProcessingSystem.Data.Repositories;

namespace OrderProcessingSystem.Data.Features.Orders;

public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IOrderRepository _repo;
    public DeleteOrderHandler(IOrderRepository repo) => _repo = repo;

    public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        await _repo.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
