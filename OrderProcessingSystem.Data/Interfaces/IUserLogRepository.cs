using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Interfaces;

public interface IUserLogRepository
{
    Task<int> AddUserLogAsync(UserLog userLog);
    Task<List<UserLog>> GetUserLogsAsync(int pageNumber, int pageSize);
    Task<List<UserLog>> GetUserLogsByEventAsync(string eventType, int pageNumber, int pageSize);
    Task<int> GetUserLogCountAsync();
    Task<int> GetUserLogCountByEventAsync(string eventType);
    Task<UserLog?> GetUserLogByIdAsync(int id);
}
