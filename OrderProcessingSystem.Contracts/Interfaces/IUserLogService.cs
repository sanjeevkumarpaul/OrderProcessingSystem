using OrderProcessingSystem.Contracts.Dto;

namespace OrderProcessingSystem.Contracts.Interfaces;

public interface IUserLogService
{
    Task<LoginEventResponse> LogLoginEventAsync(LoginEventRequest request);
    Task<UserLogListResponse> GetUserLogsAsync(int pageNumber = 1, int pageSize = 50, string? eventType = null);
}
