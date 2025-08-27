using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Repositories;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRS.Suppliers;

public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, Supplier>
{
    private readonly ISupplierRepository _repo;
    public CreateSupplierHandler(ISupplierRepository repo) => _repo = repo;

    public async Task<Supplier> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        await _repo.AddAsync(request.Supplier, cancellationToken);
        return request.Supplier;
    }
}
