using MediatR;

namespace OrderProcessingSystem.Data.MediatorCQRS.Suppliers;

public record DeleteSupplierCommand(int Id) : IRequest;
