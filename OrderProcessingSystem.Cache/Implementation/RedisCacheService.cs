using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using OrderProcessingSystem.Cache.Interfaces;

namespace OrderProcessingSystem.Cache.Implementation;

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

    public async Task SetAsync<T>(string key, T value, System.TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await SetStringAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var json = await GetStringAsync(key);
        if (string.IsNullOrEmpty(json))
            return null;
        
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var value = await GetStringAsync(key);
        return !string.IsNullOrEmpty(value);
    }
}
