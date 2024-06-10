using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using pandx.Wheel.Miscellaneous;

namespace pandx.Wheel.Caching.RedisCache;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _expiration;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly ISerializerService _serializer;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger, ISerializerService serializer,IOptions<CacheSettings> cacheSettings)
    {
        _cache = cache;
        _logger = logger;
        _serializer = serializer;
        _expiration= TimeSpan.FromMinutes(cacheSettings.Value.SlidingExpirationInMinutes);
    }

    public T? Get<T>(string key)
    {
        byte[]? data;
        try
        {
            data = _cache.Get(key);
        }
        catch
        {
            data = null;
        }

        return data is null ? default : _serializer.Deserialize<T>(Encoding.Default.GetString(data));
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        byte[]? data;
        try
        {
            data = await _cache.GetAsync(key, token);
        }
        catch
        {
            data = null;
        }

        return data is null ? default : _serializer.Deserialize<T>(Encoding.Default.GetString(data));
    }

    public void Refresh(string key)
    {
        try
        {
            _cache.Refresh(key);
            _logger.LogDebug($"分布式缓冲 {key} 已刷新");
        }
        catch
        {
            // ignored
        }
    }

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        try
        {
            _cache.RefreshAsync(key, token);
            _logger.LogDebug($"分布式缓冲 {key} 已刷新");
        }
        catch
        {
            // ignored
        }

        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        try
        {
            _cache.Remove(key);
            _logger.LogDebug($"分布式缓冲 {key} 已删除");
        }
        catch
        {
            // ignored
        }
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        try
        {
            _cache.RemoveAsync(key, token);
            _logger.LogDebug($"分布式缓冲 {key} 已删除");
        }
        catch
        {
            // ignored
        }

        return Task.CompletedTask;
    }

    public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
    {
        try
        {
            _cache.Set(key, Encoding.Default.GetBytes(_serializer.Serialize(value)), new DistributedCacheEntryOptions
            {
                SlidingExpiration = slidingExpiration ?? _expiration
            });
            _logger.LogDebug($"分布式缓冲 {key} 已添加");
        }
        catch
        {
            //ignored
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null,
        CancellationToken token = default)
    {
        try
        {
            await _cache.SetAsync(key, Encoding.Default.GetBytes(_serializer.Serialize(value)),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = slidingExpiration ?? _expiration
                }, token);
            _logger.LogDebug($"分布式缓冲 {key} 已添加");
        }
        catch
        {
            //ignored
        }
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
            _logger.LogDebug($"分布式缓冲 {key} 已添加");
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
            _logger.LogDebug($"分布式缓冲 {key} 已添加");
        }

        return value;
    }
}