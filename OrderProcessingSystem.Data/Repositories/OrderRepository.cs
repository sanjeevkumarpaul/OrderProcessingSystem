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

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    private readonly ISqlProvider _sqlProvider;
    public OrderRepository(AppDbContext db, ISqlProvider sqlProvider) => (_db, _sqlProvider) = (db, sqlProvider);

    public async Task<List<Order>> GetAllAsync(CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Orders.SelectAll");

        using var conn = _db.Database.GetDbConnection();
        if (conn.State == ConnectionState.Closed)
            await conn.OpenAsync(ct);

        var lookup = new Dictionary<int, Order>();
    var result = await conn.QueryAsync<Order, Supplier, Customer, Order>(sql,
            (order, supplier, customer) =>
            {
                if (!lookup.TryGetValue(order.OrderId, out var existing))
                {
                    existing = order;
                    existing.Supplier = supplier;
                    existing.Customer = customer;
                    lookup.Add(existing.OrderId, existing);
                }
                return existing;
        }, splitOn: "SupplierId,CustomerId");

        return lookup.Values.AsList();
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Orders.SelectById");

        using var conn = _db.Database.GetDbConnection();
        if (conn.State == ConnectionState.Closed)
            await conn.OpenAsync(ct);

    var result = await conn.QueryAsync<Order, Supplier, Customer, Order>(sql, (order, supplier, customer) =>
        {
            order.Supplier = supplier;
            order.Customer = customer;
            return order;
    }, new { Id = id }, splitOn: "SupplierId,CustomerId");

        return result.FirstOrDefault();
    }

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Orders.Insert");
    var id = await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteScalarLongAsync(_db, sql, new { order.CustomerId, order.Status, order.SupplierId, order.Total }, ct);
    order.OrderId = (int)id;
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Orders.Update");
    await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteAsync(_db, sql, order, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Orders.Delete");
    await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteAsync(_db, sql, new { Id = id }, ct);
    }

    public async Task<List<Order>> GetOrdersByCustomerAndSupplierAsync(int customerId, int supplierId, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Orders.SelectByCustomerAndSupplier");

        using var conn = _db.Database.GetDbConnection();
        if (conn.State == ConnectionState.Closed)
            await conn.OpenAsync(ct);

        var result = await conn.QueryAsync<Order, Supplier, Customer, Order>(sql,
            (order, supplier, customer) =>
            {
                order.Supplier = supplier;
                order.Customer = customer;
                return order;
            }, new { CustomerId = customerId, SupplierId = supplierId }, splitOn: "SupplierId,CustomerId");

        return result.ToList();
    }
}
