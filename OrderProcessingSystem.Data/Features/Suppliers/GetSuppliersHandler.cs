using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Repositories;

namespace OrderProcessingSystem.Data.Features.Suppliers;

public class GetSuppliersHandler : IRequestHandler<GetSuppliersQuery, List<Supplier>>
{
    private readonly ISupplierRepository _repo;
    public GetSuppliersHandler(ISupplierRepository repo) => _repo = repo;

    public async Task<List<Supplier>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetAllAsync(cancellationToken);
    }
}
