using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;
using OrderProcessingSystem.Events.Models;

namespace OrderProcessingSystem.Data.MediatorCQRSFeature.Orders;

public class ProcessOrderTransactionHandler : IRequestHandler<ProcessOrderTransactionCommand, Order>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ICustomerRepository _customerRepository;

    public ProcessOrderTransactionHandler(
        IOrderRepository orderRepository,
        ISupplierRepository supplierRepository,
        ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _supplierRepository = supplierRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Order> Handle(ProcessOrderTransactionCommand request, CancellationToken cancellationToken)
    {
        var orderTransaction = request.OrderTransaction;

        // Step 1: Find or create Supplier
        var supplier = await GetOrCreateSupplierAsync(orderTransaction.Supplier, cancellationToken);

        // Step 2: Find or create Customer
        var customer = await GetOrCreateCustomerAsync(orderTransaction.Customer, cancellationToken);

        // Step 3: Create Order
        var order = new Order
        {
            CustomerId = customer.CustomerId,
            SupplierId = supplier.SupplierId,
            Total = (double)orderTransaction.Supplier.Price * orderTransaction.Supplier.Quantity,
            Status = "Pending",
            Customer = customer,
            Supplier = supplier
        };

        await _orderRepository.AddAsync(order, cancellationToken);
        return order;
    }

    private async Task<Supplier> GetOrCreateSupplierAsync(SupplierInfoSchema supplierInfo, CancellationToken cancellationToken)
    {
        // Try to find existing supplier by name
        var existingSupplier = await _supplierRepository.GetByNameAsync(supplierInfo.Name, cancellationToken);
        
        if (existingSupplier != null)
        {
            return existingSupplier;
        }

        // Create new supplier if not found
        var newSupplier = new Supplier
        {
            Name = supplierInfo.Name,
            Country = "Unknown" // Default value since not provided in OrderTransaction
        };

        await _supplierRepository.AddAsync(newSupplier, cancellationToken);
        return newSupplier;
    }

    private async Task<Customer> GetOrCreateCustomerAsync(CustomerInfoSchema customerInfo, CancellationToken cancellationToken)
    {
        // Try to find existing customer by name
        var existingCustomer = await _customerRepository.GetByNameAsync(customerInfo.Name, cancellationToken);
        
        if (existingCustomer != null)
        {
            return existingCustomer;
        }

        // Create new customer if not found
        var newCustomer = new Customer
        {
            Name = customerInfo.Name
        };

        await _customerRepository.AddAsync(newCustomer, cancellationToken);
        return newCustomer;
    }
}
