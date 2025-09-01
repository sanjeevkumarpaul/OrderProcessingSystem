using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.Repositories;

public class TransExceptionRepository : ITransExceptionRepository
{
    private readonly AppDbContext _db;

    public TransExceptionRepository(AppDbContext db) => _db = db;

    public async Task<List<TransException>> GetAllAsync(CancellationToken ct = default)
    {
        var sql = @"SELECT TransExceptionId, TransactionType, InputMessage, Reason, RunTime
                    FROM TransExceptions
                    ORDER BY RunTime DESC";

        return await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<TransException>(_db, sql, null, ct);
    }

    public async Task<TransException?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var sql = @"SELECT TransExceptionId, TransactionType, InputMessage, Reason, RunTime
                    FROM TransExceptions
                    WHERE TransExceptionId = @Id";

        return await OrderProcessingSystem.Data.Common.DapperExecutor.QuerySingleOrDefaultAsync<TransException>(_db, sql, new { Id = id }, ct);
    }

    public async Task AddAsync(TransException transException, CancellationToken ct = default)
    {
        var sql = @"INSERT INTO TransExceptions (TransactionType, InputMessage, Reason, RunTime) 
                    VALUES (@TransactionType, @InputMessage, @Reason, @RunTime); 
                    SELECT last_insert_rowid();";

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
        var sql = @"SELECT TransExceptionId, TransactionType, InputMessage, Reason, RunTime
                    FROM TransExceptions
                    WHERE TransactionType = @TransactionType
                    ORDER BY RunTime DESC";

        return await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<TransException>(_db, sql, new { TransactionType = transactionType }, ct);
    }

    public async Task<List<TransException>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var sql = @"SELECT TransExceptionId, TransactionType, InputMessage, Reason, RunTime
                    FROM TransExceptions
                    WHERE RunTime BETWEEN @StartDate AND @EndDate
                    ORDER BY RunTime DESC";

        return await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<TransException>(_db, sql, new { StartDate = startDate, EndDate = endDate }, ct);
    }
}
