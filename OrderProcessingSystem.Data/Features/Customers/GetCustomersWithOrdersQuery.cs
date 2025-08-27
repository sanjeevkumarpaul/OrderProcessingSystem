using MediatR;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Data.Features.Customers;

public record GetCustomersWithOrdersQuery : IRequest<List<CustomerWithOrdersDto>>;
