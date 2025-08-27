using MediatR;
using OrderProcessingSystem.Data.Common;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Data.MediatorCQRS.Suppliers;

public class GetSuppliersWithOrdersHandler : IRequestHandler<GetSuppliersWithOrdersQuery, List<SupplierWithOrdersDto>>
{
    private readonly AppDbContext _db;
    
    public GetSuppliersWithOrdersHandler(AppDbContext db) => _db = db;

    public async Task<List<SupplierWithOrdersDto>> Handle(GetSuppliersWithOrdersQuery request, CancellationToken cancellationToken)
    {
        // Efficient single SQL query with LEFT JOIN to calculate statistics
        var sql = """
            SELECT 
                s.SupplierId,
                s.Name,
                s.Country,
                COALESCE(COUNT(o.OrderId), 0) as OrdersSupplied
            FROM Suppliers s
            LEFT JOIN Orders o ON s.SupplierId = o.SupplierId
            GROUP BY s.SupplierId, s.Name, s.Country
            ORDER BY s.Name
            """;
            
        return await DapperExecutor.QueryAsync<SupplierWithOrdersDto>(_db, sql, null, cancellationToken);
    }
}
