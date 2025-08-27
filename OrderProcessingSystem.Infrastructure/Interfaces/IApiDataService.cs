using System.Collections.Generic;
using System.Threading.Tasks;
using OrderProcessingSystem.Data.Features.Customers;
using OrderProcessingSystem.Data.Features.Suppliers;
using ContractsDto = OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Infrastructure.Interfaces
{
    public interface IApiDataService
    {
        Task<List<ContractsDto.OrderDto>> GetOrdersAsync();
        Task<List<ContractsDto.SupplierDto>> GetSuppliersAsync();
        Task<List<ContractsDto.CustomerDto>> GetCustomersAsync();
        Task<List<ContractsDto.SalesByCustomerDto>> GetSalesByCustomerAsync(int? customerId = null, int? top = null);
        
        // New optimized methods with database-level calculations
        Task<List<CustomerWithOrdersDto>> GetCustomersWithOrdersAsync();
        Task<List<SupplierWithOrdersDto>> GetSuppliersWithOrdersAsync();
    }
}
