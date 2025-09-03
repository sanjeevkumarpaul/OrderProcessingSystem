using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Interfaces;

namespace OrderProcessingSystem.Data.Repositories;

public class UserLogRepository : IUserLogRepository
{
    private readonly AppDbContext _context;

    public UserLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddUserLogAsync(UserLog userLog)
    {
        _context.UserLogs.Add(userLog);
        await _context.SaveChangesAsync();
        return userLog.Id;
    }

    public async Task<List<UserLog>> GetUserLogsAsync(int pageNumber = 1, int pageSize = 50)
    {
        return await _context.UserLogs
            .OrderByDescending(ul => ul.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<UserLog>> GetUserLogsByEventAsync(string eventType, int pageNumber = 1, int pageSize = 50)
    {
        return await _context.UserLogs
            .Where(ul => ul.Event == eventType)
            .OrderByDescending(ul => ul.EventDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetUserLogCountAsync()
    {
        return await _context.UserLogs.CountAsync();
    }

    public async Task<int> GetUserLogCountByEventAsync(string eventType)
    {
        return await _context.UserLogs.CountAsync(ul => ul.Event == eventType);
    }

    public async Task<UserLog?> GetUserLogByIdAsync(int id)
    {
        return await _context.UserLogs.FirstOrDefaultAsync(ul => ul.Id == id);
    }
}
