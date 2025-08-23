using MediatR;
using OrderProcessingSystem.Data.Repositories;

namespace OrderProcessingSystem.Data.Features.Suppliers;

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
