using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Customer customer, CancellationToken ct = default);
}

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;
    private readonly OrderProcessingSystem.Core.Sql.ISqlProvider _sqlProvider;
    public CustomerRepository(AppDbContext db, OrderProcessingSystem.Core.Sql.ISqlProvider sqlProvider) => (_db, _sqlProvider) = (db, sqlProvider);

    public async Task<List<Customer>> GetAllAsync(CancellationToken ct = default)
    {
    var sql = _sqlProvider.GetSql("Customers.SelectAll");
    return await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<Customer>(_db, sql, null, ct);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Customers.Insert");
        var id = await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteScalarLongAsync(_db, sql, new { customer.Name }, ct);
        customer.CustomerId = (int)id;
    }
}
