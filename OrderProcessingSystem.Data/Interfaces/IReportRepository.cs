using OrderProcessingSystem.Data.MediatorCQRS.Reports;

namespace OrderProcessingSystem.Data.Interfaces;

public interface IReportRepository
{
    Task<List<SalesByCustomerDto>> GetSalesByCustomerAsync(int? customerId = null, int? top = null, CancellationToken ct = default);
}
