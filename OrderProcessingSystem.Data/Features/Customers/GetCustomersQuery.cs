using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Features.Customers;

public record GetCustomersQuery : IRequest<List<Customer>>;
