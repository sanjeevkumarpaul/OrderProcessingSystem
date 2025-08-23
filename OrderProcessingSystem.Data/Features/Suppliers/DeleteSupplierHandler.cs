using MediatR;
using OrderProcessingSystem.Data.Repositories;

namespace OrderProcessingSystem.Data.Features.Suppliers;

public class DeleteSupplierHandler : IRequestHandler<DeleteSupplierCommand>
{
    private readonly ISupplierRepository _repo;
    public DeleteSupplierHandler(ISupplierRepository repo) => _repo = repo;

    public async Task<Unit> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        await _repo.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
