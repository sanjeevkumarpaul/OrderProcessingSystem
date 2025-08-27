using MediatR;
using AutoMapper;
using OrderProcessingSystem.Data.MediatorCQRS.Orders;
using OrderProcessingSystem.Data.MediatorCQRS.Suppliers;
using OrderProcessingSystem.Data.MediatorCQRS.Customers;
using OrderProcessingSystem.Data.MediatorCQRS.Reports;
using OrderProcessingSystem.Contracts.Interfaces;
using ContractsDto = OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Infrastructure.Services
{
    public class ApiDataService : IApiDataService
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ApiDataService(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<List<ContractsDto.OrderDto>> GetOrdersAsync()
        {
            var orders = await _mediator.Send(new GetOrdersQuery());
            return _mapper.Map<List<ContractsDto.OrderDto>>(orders);
        }

        public async Task<List<ContractsDto.SupplierDto>> GetSuppliersAsync()
        {
            var suppliers = await _mediator.Send(new GetSuppliersQuery());
            return _mapper.Map<List<ContractsDto.SupplierDto>>(suppliers);
        }

        public async Task<List<ContractsDto.CustomerDto>> GetCustomersAsync()
        {
            var customers = await _mediator.Send(new GetCustomersQuery());
            return _mapper.Map<List<ContractsDto.CustomerDto>>(customers);
        }

        public async Task<List<ContractsDto.SalesByCustomerDto>> GetSalesByCustomerAsync(int? customerId = null, int? top = null)
        {
            var query = new SalesByCustomerQuery
            {
                CustomerId = customerId,
                Top = top
            };
            var report = await _mediator.Send(query);
            // Map from Data.MediatorCQRS.Reports.SalesByCustomerDto to Contracts.Dto.SalesByCustomerDto
            return _mapper.Map<List<ContractsDto.SalesByCustomerDto>>(report);
        }

        public async Task<List<ContractsDto.CustomerWithOrdersDto>> GetCustomersWithOrdersAsync()
        {
            var customers = await _mediator.Send(new GetCustomersWithOrdersQuery());
            return _mapper.Map<List<ContractsDto.CustomerWithOrdersDto>>(customers);
        }

        public async Task<List<ContractsDto.SupplierWithOrdersDto>> GetSuppliersWithOrdersAsync()
        {
            var suppliers = await _mediator.Send(new GetSuppliersWithOrdersQuery());
            return _mapper.Map<List<ContractsDto.SupplierWithOrdersDto>>(suppliers);
        }
    }
}
