using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRSFeature.TransExceptions;

public class CreateTransExceptionHandler : IRequestHandler<CreateTransExceptionCommand, int>
{
    private readonly ITransExceptionRepository _repository;

    public CreateTransExceptionHandler(ITransExceptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateTransExceptionCommand request, CancellationToken cancellationToken)
    {
        var transException = new TransException
        {
            TransactionType = request.TransactionType,
            InputMessage = request.InputMessage,
            Reason = request.Reason,
            RunTime = DateTime.UtcNow
        };

        await _repository.AddAsync(transException, cancellationToken);
        return transException.TransExceptionId;
    }
}
