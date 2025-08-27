using MediatR;

namespace OrderProcessingSystem.Data.MediatorCQRS.Reports;

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
