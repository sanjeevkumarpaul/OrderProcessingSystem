using MediatR;
using OrderProcessingSystem.Data.Repositories;

namespace OrderProcessingSystem.Data.Features.Reports;

public class SalesByCustomerHandler : IRequestHandler<SalesByCustomerQuery, List<SalesByCustomerDto>>
{
    private readonly IReportRepository _reports;
    public SalesByCustomerHandler(IReportRepository reports) => _reports = reports;

    public async Task<List<SalesByCustomerDto>> Handle(SalesByCustomerQuery request, CancellationToken cancellationToken)
    {
        return await _reports.GetSalesByCustomerAsync(cancellationToken);
    }
}
