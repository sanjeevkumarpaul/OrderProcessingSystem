using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Features.Reports;
using Dapper;

namespace OrderProcessingSystem.Data.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _db;
    public ReportRepository(AppDbContext db) => _db = db;

    public async Task<List<SalesByCustomerDto>> GetSalesByCustomerAsync(CancellationToken ct = default)
    {
        var sql = @"
SELECT
    o.CustomerId as CustomerId,
    COALESCE(c.Name, '') as CustomerName,
    SUM(o.Total) as TotalSales,
    COUNT(1) as OrderCount
FROM Orders o
LEFT JOIN Customers c ON c.CustomerId = o.CustomerId
GROUP BY o.CustomerId, c.Name;";
    return await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<SalesByCustomerDto>(_db, sql, null, ct);
    }
}
