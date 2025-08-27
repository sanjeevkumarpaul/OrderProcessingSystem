using MediatR;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Data.Features.Suppliers;

public record GetSuppliersWithOrdersQuery : IRequest<List<SupplierWithOrdersDto>>;
