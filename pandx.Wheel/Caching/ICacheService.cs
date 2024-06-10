using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Caching;

public interface ICacheService : IScopedDependency
{
    T? Get<T>(string key);
    Task<T?> GetAsync<T>(string key, CancellationToken token = default);

    void Refresh(string key);
    Task RefreshAsync(string key, CancellationToken token = default);

    void Remove(string key);
    Task RemoveAsync(string key, CancellationToken token = default);

    void Set<T>(string key, T value, TimeSpan? slidingExpiration = null);

    Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null,
        CancellationToken token = default);

    T? GetOrSet<T>(string key, Func<T?> callback, TimeSpan? slidingExpiration = null);

    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> callback, TimeSpan? slidingExpiration = null,
        CancellationToken token = default);
}