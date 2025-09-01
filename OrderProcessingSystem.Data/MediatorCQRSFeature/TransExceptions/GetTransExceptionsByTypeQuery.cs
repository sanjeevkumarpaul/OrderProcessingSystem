using MediatR;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.MediatorCQRSFeature.TransExceptions;

public record GetTransExceptionsByTypeQuery(string TransactionType) : IRequest<List<TransException>>;
