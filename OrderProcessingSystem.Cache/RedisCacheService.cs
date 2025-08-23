using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace OrderProcessingSystem.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    public RedisCacheService(IDistributedCache cache) => _cache = cache;

    public async Task SetStringAsync(string key, string value, System.TimeSpan? expiry = null)
    {
        var opts = new DistributedCacheEntryOptions();
        if (expiry.HasValue) opts.SetAbsoluteExpiration(expiry.Value);
        await _cache.SetStringAsync(key, value, opts);
    }

    public async Task<string?> GetStringAsync(string key)
        => await _cache.GetStringAsync(key);
}
