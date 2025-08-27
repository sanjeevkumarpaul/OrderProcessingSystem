using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.MediatorCQRS.Suppliers;

public record UpdateSupplierCommand(Supplier Supplier) : IRequest;
