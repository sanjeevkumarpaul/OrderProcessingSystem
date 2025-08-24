using OrderProcessingSystem.Data.Features.Reports;

namespace OrderProcessingSystem.Data.Repositories;

public interface IReportRepository
{
    Task<List<SalesByCustomerDto>> GetSalesByCustomerAsync(int? customerId = null, int? top = null, CancellationToken ct = default);
}
