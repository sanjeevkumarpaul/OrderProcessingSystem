using MediatR;

namespace OrderProcessingSystem.Data.Features.Reports;

public record SalesByCustomerQuery : IRequest<List<SalesByCustomerDto>>;
