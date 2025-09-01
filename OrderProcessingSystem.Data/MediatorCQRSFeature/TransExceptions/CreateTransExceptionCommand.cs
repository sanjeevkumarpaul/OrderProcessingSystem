using MediatR;

namespace OrderProcessingSystem.Data.MediatorCQRSFeature.TransExceptions;

public record CreateTransExceptionCommand(
    string TransactionType,
    string InputMessage,
    string Reason
) : IRequest<int>;
