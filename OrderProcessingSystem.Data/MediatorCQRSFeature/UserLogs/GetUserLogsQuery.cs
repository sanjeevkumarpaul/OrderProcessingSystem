using MediatR;
using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Data.MediatorCQRS.UserLogs;

public class GetUserLogsQuery : IRequest<(List<UserLogDto> logs, int totalCount)>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? UserId { get; set; }
    public string? EventType { get; set; }
}
