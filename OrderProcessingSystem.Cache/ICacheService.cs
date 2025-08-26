using System.Threading.Tasks;

namespace OrderProcessingSystem.Cache;

public interface ICacheService
{
    Task SetStringAsync(string key, string value, System.TimeSpan? expiry = null);
    Task<string?> GetStringAsync(string key);
    
    // Generic methods for complex objects
    Task SetAsync<T>(string key, T value, System.TimeSpan? expiry = null);
    Task<T?> GetAsync<T>(string key) where T : class;
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}
