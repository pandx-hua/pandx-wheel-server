namespace pandx.Wheel.Caching;

public class CacheKeyService : ICacheKeyService
{
    public string GetCacheKey(string token, string name)
    {
        return $"{token}-{name}";
    }

    public string GetCacheKey(string token)
    {
        return $"{token}";
    }
}