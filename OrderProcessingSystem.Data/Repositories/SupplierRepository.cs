using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;
using OrderProcessingSystem.Contracts.Interfaces;

namespace OrderProcessingSystem.Data.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _db;
    private readonly ISqlProvider _sqlProvider;
    public SupplierRepository(AppDbContext db, ISqlProvider sqlProvider) => (_db, _sqlProvider) = (db, sqlProvider);

    public async Task<List<Supplier>> GetAllAsync(CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Suppliers.SelectAll");
    return await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<Supplier>(_db, sql, null, ct);
    }

    public async Task<Supplier?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Suppliers.SelectById");
    return await OrderProcessingSystem.Data.Common.DapperExecutor.QuerySingleOrDefaultAsync<Supplier>(_db, sql, new { Id = id }, ct);
    }

    public async Task<Supplier?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Suppliers.SelectByName");
    return await OrderProcessingSystem.Data.Common.DapperExecutor.QuerySingleOrDefaultAsync<Supplier>(_db, sql, new { Name = name }, ct);
    }

    public async Task AddAsync(Supplier supplier, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Suppliers.Insert");
    var id = await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteScalarLongAsync(_db, sql, new { supplier.Name, supplier.Country }, ct);
    supplier.SupplierId = (int)id;
    }

    public async Task UpdateAsync(Supplier supplier, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Suppliers.Update");
    await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteAsync(_db, sql, supplier, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Suppliers.Delete");
    await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteAsync(_db, sql, new { Id = id }, ct);
    }
}
