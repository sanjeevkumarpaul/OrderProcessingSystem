using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Repositories;

namespace OrderProcessingSystem.Data.Features.Customers;

public class GetCustomersHandler : IRequestHandler<GetCustomersQuery, List<Customer>>
{
    private readonly ICustomerRepository _repo;
    public GetCustomersHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<List<Customer>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetAllAsync(cancellationToken);
    }
}
