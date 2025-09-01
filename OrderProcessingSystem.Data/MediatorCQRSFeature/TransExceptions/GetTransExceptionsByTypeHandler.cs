using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRSFeature.TransExceptions;

public class GetTransExceptionsByTypeHandler : IRequestHandler<GetTransExceptionsByTypeQuery, List<TransException>>
{
    private readonly ITransExceptionRepository _repository;

    public GetTransExceptionsByTypeHandler(ITransExceptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TransException>> Handle(GetTransExceptionsByTypeQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByTransactionTypeAsync(request.TransactionType, cancellationToken);
    }
}
