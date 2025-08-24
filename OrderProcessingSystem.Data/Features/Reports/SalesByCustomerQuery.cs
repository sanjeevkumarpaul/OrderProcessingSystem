using MediatR;

namespace OrderProcessingSystem.Data.Features.Reports;

public record SalesByCustomerQuery : IRequest<List<SalesByCustomerDto>>
{
	public int? CustomerId { get; set; }
	public int? Top { get; set; }

	public SalesByCustomerQuery() { }
	public SalesByCustomerQuery(int? customerId, int? top)
	{
		CustomerId = customerId;
		Top = top;
	}
}
