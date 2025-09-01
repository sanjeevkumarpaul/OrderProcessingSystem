using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.MediatorCQRSFeature.TransExceptions;

public record GetAllTransExceptionsQuery() : IRequest<List<TransException>>;
