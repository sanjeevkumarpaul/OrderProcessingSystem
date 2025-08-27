using System.Collections.Generic;
using System.Threading.Tasks;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Contracts.Interfaces;

public interface IApiDataService
{
    Task<List<OrderDto>> GetOrdersAsync();
    Task<List<SupplierDto>> GetSuppliersAsync();
    Task<List<CustomerDto>> GetCustomersAsync();
    Task<List<SalesByCustomerDto>> GetSalesByCustomerAsync(int? customerId = null, int? top = null);
    
    // New optimized methods with database-level calculations
    Task<List<CustomerWithOrdersDto>> GetCustomersWithOrdersAsync();
    Task<List<SupplierWithOrdersDto>> GetSuppliersWithOrdersAsync();
}
