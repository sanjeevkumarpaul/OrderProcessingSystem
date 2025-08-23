using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _db;
    public SupplierRepository(AppDbContext db) => _db = db;

    public async Task<List<Supplier>> GetAllAsync(CancellationToken ct = default)
    {
    var sql = "SELECT SupplierId, Name, Country FROM Suppliers";
    return await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<Supplier>(_db, sql, null, ct);
    }

    public async Task<Supplier?> GetByIdAsync(int id, CancellationToken ct = default)
    {
    var sql = "SELECT SupplierId, Name, Country FROM Suppliers WHERE SupplierId = @Id";
    return await OrderProcessingSystem.Data.Common.DapperExecutor.QuerySingleOrDefaultAsync<Supplier>(_db, sql, new { Id = id }, ct);
    }

    public async Task AddAsync(Supplier supplier, CancellationToken ct = default)
    {
    var sql = "INSERT INTO Suppliers (Name, Country) VALUES (@Name, @Country); SELECT last_insert_rowid();";
    var id = await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteScalarLongAsync(_db, sql, new { supplier.Name, supplier.Country }, ct);
    supplier.SupplierId = (int)id;
    }

    public async Task UpdateAsync(Supplier supplier, CancellationToken ct = default)
    {
    var sql = "UPDATE Suppliers SET Name = @Name, Country = @Country WHERE SupplierId = @SupplierId";
    await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteAsync(_db, sql, supplier, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
    var sql = "DELETE FROM Suppliers WHERE SupplierId = @Id";
    await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteAsync(_db, sql, new { Id = id }, ct);
    }
}
