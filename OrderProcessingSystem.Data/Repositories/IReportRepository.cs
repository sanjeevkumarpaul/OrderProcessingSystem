using OrderProcessingSystem.Data.Features.Reports;

namespace OrderProcessingSystem.Data.Repositories;

public interface IReportRepository
{
    Task<List<SalesByCustomerDto>> GetSalesByCustomerAsync(CancellationToken ct = default);
}
