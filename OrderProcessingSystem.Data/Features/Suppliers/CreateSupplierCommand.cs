using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Features.Suppliers;

public record CreateSupplierCommand(Supplier Supplier) : IRequest<Supplier>;
