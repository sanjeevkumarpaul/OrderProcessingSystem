using System.Collections.Generic;
using System.Threading.Tasks;
using OrderProcessingSystem.Infrastructure.Interfaces;
using MediatR;
using OrderProcessingSystem.Data.Features.Orders;
using OrderProcessingSystem.Data.Features.Suppliers;
using OrderProcessingSystem.Data.Features.Customers;
using OrderProcessingSystem.Data.Features.Reports;
// using Data.Features.* so the service can send queries via IMediator

namespace OrderProcessingSystem.Infrastructure.Services
{
    public class ApiDataService : IApiDataService
    {
        
    private readonly IMediator _mediator;

        public ApiDataService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<System.Collections.Generic.List<OrderProcessingSystem.Data.Entities.Order>> GetOrdersAsync()
        {
            var orders = await _mediator.Send(new OrderProcessingSystem.Data.Features.Orders.GetOrdersQuery());
            return orders;
        }

        public async Task<System.Collections.Generic.List<OrderProcessingSystem.Data.Entities.Supplier>> GetSuppliersAsync()
        {
            var suppliers = await _mediator.Send(new OrderProcessingSystem.Data.Features.Suppliers.GetSuppliersQuery());
            return suppliers;
        }

        public async Task<System.Collections.Generic.List<OrderProcessingSystem.Data.Entities.Customer>> GetCustomersAsync()
        {
            var customers = await _mediator.Send(new OrderProcessingSystem.Data.Features.Customers.GetCustomersQuery());
            return customers;
        }

    public async Task<System.Collections.Generic.List<OrderProcessingSystem.Data.Features.Reports.SalesByCustomerDto>> GetSalesByCustomerAsync(int? customerId = null, int? top = null)
        {
            var query = new OrderProcessingSystem.Data.Features.Reports.SalesByCustomerQuery
            {
                CustomerId = customerId,
                Top = top
            };
            var report = await _mediator.Send(query);
            return report;
        }
    }
}
