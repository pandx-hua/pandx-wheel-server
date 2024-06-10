using pandx.Wheel.DependencyInjection;

namespace pandx.Wheel.Caching;

public interface ICacheKeyService : IScopedDependency
{
    string GetCacheKey(string token, string name);
    string GetCacheKey(string token);
}