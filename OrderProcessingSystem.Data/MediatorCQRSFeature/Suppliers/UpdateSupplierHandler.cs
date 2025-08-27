using MediatR;
using OrderProcessingSystem.Data.Repositories;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRS.Suppliers;

public class UpdateSupplierHandler : IRequestHandler<UpdateSupplierCommand>
{
    private readonly ISupplierRepository _repo;
    public UpdateSupplierHandler(ISupplierRepository repo) => _repo = repo;

    public async Task<Unit> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        await _repo.UpdateAsync(request.Supplier, cancellationToken);
        return Unit.Value;
    }
}
