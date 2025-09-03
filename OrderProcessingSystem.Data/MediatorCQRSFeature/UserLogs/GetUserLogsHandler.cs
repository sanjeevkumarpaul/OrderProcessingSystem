using MediatR;
using OrderProcessingSystem.Contracts.Dto;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.MediatorCQRS.UserLogs;

public class GetUserLogsHandler : IRequestHandler<GetUserLogsQuery, (List<UserLogDto> logs, int totalCount)>
{
    private readonly IUserLogRepository _repo;
    
    public GetUserLogsHandler(IUserLogRepository repo) => _repo = repo;

    public async Task<(List<UserLogDto> logs, int totalCount)> Handle(GetUserLogsQuery request, CancellationToken cancellationToken)
    {
        List<UserLog> logs;
        int totalCount;

        if (!string.IsNullOrEmpty(request.EventType))
        {
            logs = await _repo.GetUserLogsByEventAsync(request.EventType.ToUpper(), request.PageNumber, request.PageSize);
            totalCount = await _repo.GetUserLogCountByEventAsync(request.EventType.ToUpper());
        }
        else
        {
            logs = await _repo.GetUserLogsAsync(request.PageNumber, request.PageSize);
            totalCount = await _repo.GetUserLogCountAsync();
        }

        // Filter by UserId if provided (simple contains filter)
        if (!string.IsNullOrEmpty(request.UserId))
        {
            logs = logs.Where(l => l.UserId.Contains(request.UserId, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var logDtos = logs.Select(log => new UserLogDto
        {
            Id = log.Id,
            EventDate = log.EventDate,
            Event = log.Event,
            UserId = log.UserId,
            UserName = log.UserName
        }).ToList();
        
        return (logDtos, totalCount);
    }
}
