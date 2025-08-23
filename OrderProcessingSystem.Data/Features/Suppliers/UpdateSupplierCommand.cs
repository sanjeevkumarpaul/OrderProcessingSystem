using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Features.Suppliers;

public record UpdateSupplierCommand(Supplier Supplier) : IRequest;
