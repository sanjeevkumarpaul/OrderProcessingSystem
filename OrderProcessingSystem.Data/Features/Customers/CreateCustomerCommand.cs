using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Features.Customers;

public record CreateCustomerCommand(Customer Customer) : IRequest<Customer>;
