using MediatR;
using OrderProcessingSystem.Data.Repositories;
using OrderProcessingSystem.Data.Interfaces;


namespace OrderProcessingSystem.Data.MediatorCQRS.Orders;

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
