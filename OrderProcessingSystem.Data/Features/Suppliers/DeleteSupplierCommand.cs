using MediatR;

namespace OrderProcessingSystem.Data.Features.Suppliers;

public record DeleteSupplierCommand(int Id) : IRequest;
