using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;
using OrderProcessingSystem.Contracts.Interfaces;

namespace OrderProcessingSystem.Data.Repositories;

public class TransExceptionRepository : ITransExceptionRepository
{
    private readonly AppDbContext _db;
    private readonly ISqlProvider _sqlProvider;

    public TransExceptionRepository(AppDbContext db, ISqlProvider sqlProvider) => (_db, _sqlProvider) = (db, sqlProvider);

    public async Task<List<TransException>> GetAllAsync(CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("TransExceptions.SelectAll");

        return await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<TransException>(_db, sql, null, ct);
    }

    public async Task<TransException?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("TransExceptions.SelectById");

        return await OrderProcessingSystem.Data.Common.DapperExecutor.QuerySingleOrDefaultAsync<TransException>(_db, sql, new { Id = id }, ct);
    }

    public async Task AddAsync(TransException transException, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("TransExceptions.Insert");

        var id = await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteScalarLongAsync(_db, sql, new 
        { 
            transException.TransactionType, 
            transException.InputMessage, 
            transException.Reason, 
            transException.RunTime 
        }, ct);
        
        transException.TransExceptionId = (int)id;
    }

    public async Task<List<TransException>> GetByTransactionTypeAsync(string transactionType, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("TransExceptions.SelectByTransactionType");

        return await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<TransException>(_db, sql, new { TransactionType = transactionType }, ct);
    }

    public async Task<List<TransException>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("TransExceptions.SelectByDateRange");

        return await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<TransException>(_db, sql, new { StartDate = startDate, EndDate = endDate }, ct);
    }
}
