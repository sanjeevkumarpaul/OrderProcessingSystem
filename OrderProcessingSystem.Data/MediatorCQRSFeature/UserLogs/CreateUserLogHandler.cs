using MediatR;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRS.UserLogs;

public class CreateUserLogHandler : IRequestHandler<CreateUserLogCommand, UserLog>
{
    private readonly IUserLogRepository _repo;
    
    public CreateUserLogHandler(IUserLogRepository repo) => _repo = repo;

    public async Task<UserLog> Handle(CreateUserLogCommand request, CancellationToken cancellationToken)
    {
        var id = await _repo.AddUserLogAsync(request.UserLog);
        request.UserLog.Id = id;
        return request.UserLog;
    }
}
