using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Interfaces;

public interface ISupplierRepository
{
    Task<List<Supplier>> GetAllAsync(CancellationToken ct = default);
    Task<Supplier?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Supplier?> GetByNameAsync(string name, CancellationToken ct = default);
    Task AddAsync(Supplier supplier, CancellationToken ct = default);
    Task UpdateAsync(Supplier supplier, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
