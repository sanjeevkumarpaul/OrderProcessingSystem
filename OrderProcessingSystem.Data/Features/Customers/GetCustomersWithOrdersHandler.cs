using MediatR;
using OrderProcessingSystem.Data.Common;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Data.Features.Customers;

public class GetCustomersWithOrdersHandler : IRequestHandler<GetCustomersWithOrdersQuery, List<CustomerWithOrdersDto>>
{
    private readonly AppDbContext _db;
    
    public GetCustomersWithOrdersHandler(AppDbContext db) => _db = db;

    public async Task<List<CustomerWithOrdersDto>> Handle(GetCustomersWithOrdersQuery request, CancellationToken cancellationToken)
    {
        // Efficient single SQL query with LEFT JOIN to calculate statistics
        var sql = """
            SELECT 
                c.CustomerId,
                c.Name,
                '' as Country,
                '' as Email,
                '' as Phone,
                COALESCE(COUNT(o.OrderId), 0) as OrdersCount,
                COALESCE(SUM(o.Total), 0) as TotalSales
            FROM Customers c
            LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
            GROUP BY c.CustomerId, c.Name
            ORDER BY c.Name
            """;
            
        return await DapperExecutor.QueryAsync<CustomerWithOrdersDto>(_db, sql, null, cancellationToken);
    }
}
