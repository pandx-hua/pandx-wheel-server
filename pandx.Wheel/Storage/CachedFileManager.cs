using pandx.Wheel.Caching;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Folders;

namespace pandx.Wheel.Storage;

public class CachedFileManager : ICachedFileManager
{
    private readonly ICacheKeyService _cacheKeyService;
    private readonly ICacheService _cacheService;
    private readonly ICommonFolder _commonFolder;

    public CachedFileManager(ICacheService cacheService, ICacheKeyService cacheKeyService, ICommonFolder commonFolder)
    {
        _cacheService = cacheService;
        _cacheKeyService = cacheKeyService;
        _commonFolder = commonFolder;
    }

    public virtual async Task SetFileAsync(string token, string name, byte[] content)
    {
        var cacheKey = _cacheKeyService.GetCacheKey(token, name);
        await _cacheService.SetAsync(cacheKey, content, new TimeSpan(0, 0, 2, 0));
    }

    public virtual async Task<byte[]?> GetFileAsync(string token, string name)
    {
        var cacheKey = _cacheKeyService.GetCacheKey(token, name);
        return await _cacheService.GetAsync<byte[]>(cacheKey);
    }

    public virtual async Task SaveFileAsync(string token, string name)
    {
        var file = await GetFileAsync(token, name) ?? throw new WheelException("未在缓存中找到请求的文件");
        await File.WriteAllBytesAsync(
            Path.Combine(_commonFolder.FilesFolder, $"{token}-{name}"),
            file);
    }
}