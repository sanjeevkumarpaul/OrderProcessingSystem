using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    public OrderRepository(AppDbContext db) => _db = db;

    public async Task<List<Order>> GetAllAsync(CancellationToken ct = default)
    {
    var sql = @"SELECT o.OrderId, o.CustomerId, o.SupplierId, o.Total, o.Status,
               s.SupplierId, s.Name, s.Country,
               c.CustomerId, c.Name
            FROM Orders o
            LEFT JOIN Suppliers s ON s.SupplierId = o.SupplierId
            LEFT JOIN Customers c ON c.CustomerId = o.CustomerId";

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
    var sql = @"SELECT o.OrderId, o.CustomerId, o.SupplierId, o.Total, o.Status,
               s.SupplierId, s.Name, s.Country,
               c.CustomerId, c.Name
            FROM Orders o
            LEFT JOIN Suppliers s ON s.SupplierId = o.SupplierId
            LEFT JOIN Customers c ON c.CustomerId = o.CustomerId
            WHERE o.OrderId = @Id";

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
        var sql = "INSERT INTO Orders (CustomerId, Status, SupplierId, Total) VALUES (@CustomerId, @Status, @SupplierId, @Total); SELECT last_insert_rowid();";
    var id = await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteScalarLongAsync(_db, sql, new { order.CustomerId, order.Status, order.SupplierId, order.Total }, ct);
    order.OrderId = (int)id;
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        var sql = "UPDATE Orders SET CustomerId = @CustomerId, Status = @Status, SupplierId = @SupplierId, Total = @Total WHERE OrderId = @OrderId";
    await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteAsync(_db, sql, order, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var sql = "DELETE FROM Orders WHERE OrderId = @Id";
    await OrderProcessingSystem.Data.Common.DapperExecutor.ExecuteAsync(_db, sql, new { Id = id }, ct);
    }
}
