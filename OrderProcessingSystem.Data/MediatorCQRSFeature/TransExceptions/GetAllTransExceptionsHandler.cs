using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRSFeature.TransExceptions;

public class GetAllTransExceptionsHandler : IRequestHandler<GetAllTransExceptionsQuery, List<TransException>>
{
    private readonly ITransExceptionRepository _repository;

    public GetAllTransExceptionsHandler(ITransExceptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TransException>> Handle(GetAllTransExceptionsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
