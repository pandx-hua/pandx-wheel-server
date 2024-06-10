namespace pandx.Wheel.Caching;

public class CacheSettings
{
    public bool UseDistributedCache { get; set; } = false;

    public string RedisAddress { get; set; } = "localhost";
    public string RedisInstanceName { get; set; } = "Wheel";
    public int SlidingExpirationInMinutes { get; set; } = 30;
}