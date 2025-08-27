using MediatR;
using OrderProcessingSystem.Data.Repositories;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRS.Reports;

public class SalesByCustomerHandler : IRequestHandler<SalesByCustomerQuery, List<SalesByCustomerDto>>
{
    private readonly IReportRepository _reports;
    public SalesByCustomerHandler(IReportRepository reports) => _reports = reports;

    public async Task<List<SalesByCustomerDto>> Handle(SalesByCustomerQuery request, CancellationToken cancellationToken)
    {
        return await _reports.GetSalesByCustomerAsync(request.CustomerId, request.Top, cancellationToken);
    }
}
