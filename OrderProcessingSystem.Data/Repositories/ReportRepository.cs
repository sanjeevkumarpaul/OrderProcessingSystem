using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Features.Reports;
using Dapper;

namespace OrderProcessingSystem.Data.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _db;
    public ReportRepository(AppDbContext db) => _db = db;

    public async Task<List<SalesByCustomerDto>> GetSalesByCustomerAsync(int? customerId = null, int? top = null, CancellationToken ct = default)
    {
        var sql = @"
                    SELECT
                        o.CustomerId as CustomerId,
                        COALESCE(c.Name, '') as CustomerName,
                        SUM(o.Total) as TotalSales,
                        COUNT(1) as OrderCount
                    FROM Orders o
                    LEFT JOIN Customers c ON c.CustomerId = o.CustomerId
                    WHERE (@CustomerId IS NULL OR o.CustomerId = @CustomerId)
                    GROUP BY o.CustomerId, c.Name
                    ORDER BY TotalSales DESC;";
        Console.WriteLine($"Executing SQL: {sql}, with CustomerID as {customerId}");
        var param = new { CustomerId = customerId };
        var rows = await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<SalesByCustomerDto>(_db, sql, param, ct);
        // Convention: top == 0 means 'no limit'. Apply Take only when top > 0.
        if (top.HasValue && top.Value > 0)
            return rows.Take(top.Value).ToList();
        return rows;
    }
}
