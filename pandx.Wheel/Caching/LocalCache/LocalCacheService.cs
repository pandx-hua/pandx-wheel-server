using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace pandx.Wheel.Caching.LocalCache;

public class LocalCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _expiration;
    private readonly ILogger<LocalCacheService> _logger;

    public LocalCacheService(ILogger<LocalCacheService> logger, IMemoryCache cache,IOptions<CacheSettings> cacheSettings)
    {
        _logger = logger;
        _cache = cache;
        _expiration= TimeSpan.FromMinutes(cacheSettings.Value.SlidingExpirationInMinutes);
    }

    public T? Get<T>(string key)
    {
        return _cache.Get<T>(key);
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        return Task.FromResult(Get<T>(key));
    }

    public void Refresh(string key)
    {
        _cache.TryGetValue(key, out _);
        _logger.LogDebug($"本地缓冲 {key} 已刷新");
    }

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        Refresh(key);
        _logger.LogDebug($"本地缓冲 {key} 已刷新");
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _logger.LogDebug($"本地缓冲 {key} 已删除");
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        Remove(key);
        _logger.LogDebug($"本地缓冲 {key} 已删除");
        return Task.CompletedTask;
    }

    public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
    {
        slidingExpiration ??= _expiration;
        _cache.Set(key, value, new MemoryCacheEntryOptions { SlidingExpiration = slidingExpiration });
        _logger.LogDebug($"本地缓冲 {key} 已添加");
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null,
        CancellationToken token = default)
    {
        Set(key, value, slidingExpiration);
        _logger.LogDebug($"本地缓冲 {key} 已添加");
        return Task.CompletedTask;
    }

    public T? GetOrSet<T>(string key, Func<T?> callback, TimeSpan? slidingExpiration = null)
    {
        var value = Get<T>(key);
        if (value is not null)
        {
            return value;
        }

        value = callback();
        if (value is not null)
        {
            Set(key, value, slidingExpiration);
            _logger.LogDebug($"本地缓冲 {key} 已添加");
        }

        return value;
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> callback, TimeSpan? slidingExpiration = null,
        CancellationToken token = default)
    {
        var value = await GetAsync<T>(key, token);
        if (value is not null)
        {
            return value;
        }

        value = await callback();
        if (value is not null)
        {
            await SetAsync(key, value, slidingExpiration, token);
            _logger.LogDebug($"本地缓冲 {key} 已添加");
        }

        return value;
    }
}