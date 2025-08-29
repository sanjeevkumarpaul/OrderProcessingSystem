using MediatR;
using OrderProcessingSystem.Data.MediatorCQRSFeature.Orders;
using OrderProcessingSystem.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace OrderProcessingSystem.Data.MediatorCQRSFeature.Orders
{
    public class ProcessOrderCancellationHandler : IRequestHandler<ProcessOrderCancellationCommand, bool>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<ProcessOrderCancellationHandler> _logger;

        public ProcessOrderCancellationHandler(
            ICustomerRepository customerRepository,
            ISupplierRepository supplierRepository,
            IOrderRepository orderRepository,
            ILogger<ProcessOrderCancellationHandler> logger)
        {
            _customerRepository = customerRepository;
            _supplierRepository = supplierRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(ProcessOrderCancellationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Processing order cancellation for Customer: {Customer}, Supplier: {Supplier}, Quantity: {Quantity}",
                    request.OrderCancellation.Customer, request.OrderCancellation.Supplier, request.OrderCancellation.Quantity);

                // Find customer by name
                var customer = await _customerRepository.GetByNameAsync(request.OrderCancellation.Customer);
                if (customer == null)
                {
                    _logger.LogWarning("Customer '{CustomerName}' not found for cancellation", request.OrderCancellation.Customer);
                    return false;
                }

                // Find supplier by name  
                var supplier = await _supplierRepository.GetByNameAsync(request.OrderCancellation.Supplier);
                if (supplier == null)
                {
                    _logger.LogWarning("Supplier '{SupplierName}' not found for cancellation", request.OrderCancellation.Supplier);
                    return false;
                }

                // Find and delete orders matching customer and supplier
                var ordersToDelete = await _orderRepository.GetOrdersByCustomerAndSupplierAsync(customer.CustomerId, supplier.SupplierId);
                
                if (!ordersToDelete.Any())
                {
                    _logger.LogWarning("No orders found to cancel for Customer: {Customer} and Supplier: {Supplier}",
                        request.OrderCancellation.Customer, request.OrderCancellation.Supplier);
                    return false;
                }

                int deletedCount = 0;
                foreach (var order in ordersToDelete)
                {
                    await _orderRepository.DeleteAsync(order.OrderId);
                    deletedCount++;
                    _logger.LogInformation("Deleted order {OrderId} for Customer: {Customer}, Supplier: {Supplier}",
                        order.OrderId, request.OrderCancellation.Customer, request.OrderCancellation.Supplier);
                }

                _logger.LogInformation("Successfully processed order cancellation. Deleted {DeletedCount} orders for Customer: {Customer}, Supplier: {Supplier}",
                    deletedCount, request.OrderCancellation.Customer, request.OrderCancellation.Supplier);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order cancellation for Customer: {Customer}, Supplier: {Supplier}",
                    request.OrderCancellation.Customer, request.OrderCancellation.Supplier);
                throw;
            }
        }
    }
}
