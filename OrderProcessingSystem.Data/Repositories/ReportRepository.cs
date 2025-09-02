using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.MediatorCQRS.Reports;
using OrderProcessingSystem.Data.Interfaces;
using OrderProcessingSystem.Contracts.Interfaces;
using Dapper;

namespace OrderProcessingSystem.Data.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _db;
    private readonly ISqlProvider _sqlProvider;
    public ReportRepository(AppDbContext db, ISqlProvider sqlProvider) => (_db, _sqlProvider) = (db, sqlProvider);

    public async Task<List<SalesByCustomerDto>> GetSalesByCustomerAsync(int? customerId = null, int? top = null, CancellationToken ct = default)
    {
        var sql = _sqlProvider.GetSql("Reports.SalesByCustomer");
        Console.WriteLine($"Executing SQL: {sql}, with CustomerID as {customerId}");
        var param = new { CustomerId = customerId };
        var rows = await OrderProcessingSystem.Data.Common.DapperExecutor.QueryAsync<SalesByCustomerDto>(_db, sql, param, ct);
        // Convention: top == 0 means 'no limit'. Apply Take only when top > 0.
        if (top.HasValue && top.Value > 0)
            return rows.Take(top.Value).ToList();
        return rows;
    }
}
