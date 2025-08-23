using System.Threading.Tasks;

namespace OrderProcessingSystem.Cache;

public interface ICacheService
{
    Task SetStringAsync(string key, string value, System.TimeSpan? expiry = null);
    Task<string?> GetStringAsync(string key);
}
