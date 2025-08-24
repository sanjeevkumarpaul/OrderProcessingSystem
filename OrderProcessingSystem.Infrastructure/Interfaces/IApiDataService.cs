using System.Collections.Generic;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace OrderProcessingSystem.Infrastructure.Interfaces
{
    public interface IApiDataService
    {
        Task<System.Collections.Generic.List<OrderProcessingSystem.Data.Entities.Order>> GetOrdersAsync();
        Task<System.Collections.Generic.List<OrderProcessingSystem.Data.Entities.Supplier>> GetSuppliersAsync();
        Task<System.Collections.Generic.List<OrderProcessingSystem.Data.Entities.Customer>> GetCustomersAsync();
    Task<System.Collections.Generic.List<OrderProcessingSystem.Data.Features.Reports.SalesByCustomerDto>> GetSalesByCustomerAsync(int? customerId = null, int? top = null);
    }
}
