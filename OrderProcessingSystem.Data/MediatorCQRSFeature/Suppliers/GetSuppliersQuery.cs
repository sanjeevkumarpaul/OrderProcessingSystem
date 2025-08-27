using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.MediatorCQRS.Suppliers;

public record GetSuppliersQuery : IRequest<List<Supplier>>;
