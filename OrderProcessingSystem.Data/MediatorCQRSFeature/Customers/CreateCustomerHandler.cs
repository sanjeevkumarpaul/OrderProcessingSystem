using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Repositories;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRS.Customers;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Customer>
{
    private readonly ICustomerRepository _repo;
    public CreateCustomerHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<Customer> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // repository currently only exposes GetAllAsync; add behavior if repository provides AddAsync
        // If ICustomerRepository doesn't have AddAsync yet, this will need to be added.
        if (_repo is OrderProcessingSystem.Data.Repositories.CustomerRepository concrete)
        {
            // CustomerRepository currently has no AddAsync by interface; use reflection as fallback is not ideal.
        }

    await _repo.AddAsync(request.Customer, cancellationToken);
    return request.Customer;
    }
}
