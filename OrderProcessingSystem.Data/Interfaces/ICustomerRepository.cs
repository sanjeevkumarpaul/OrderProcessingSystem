using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Interfaces;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(CancellationToken ct = default);
    Task<Customer?> GetByNameAsync(string name, CancellationToken ct = default);
    Task AddAsync(Customer customer, CancellationToken ct = default);
}
