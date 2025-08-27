using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRS.Customers;

public record GetCustomersQuery : IRequest<List<Customer>>;
