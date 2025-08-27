using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.MediatorCQRS.Suppliers;

public record CreateSupplierCommand(Supplier Supplier) : IRequest<Supplier>;
