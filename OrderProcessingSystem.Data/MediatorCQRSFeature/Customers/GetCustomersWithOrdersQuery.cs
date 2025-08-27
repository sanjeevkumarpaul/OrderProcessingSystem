using MediatR;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Data.MediatorCQRS.Customers;

public record GetCustomersWithOrdersQuery : IRequest<List<CustomerWithOrdersDto>>;
