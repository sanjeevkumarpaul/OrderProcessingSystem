using MediatR;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Data.MediatorCQRS.Suppliers;

public record GetSuppliersWithOrdersQuery : IRequest<List<SupplierWithOrdersDto>>;
