using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Interfaces;

public interface ITransExceptionRepository
{
    Task<List<TransException>> GetAllAsync(CancellationToken ct = default);
    Task<TransException?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(TransException transException, CancellationToken ct = default);
    Task<List<TransException>> GetByTransactionTypeAsync(string transactionType, CancellationToken ct = default);
    Task<List<TransException>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
}
