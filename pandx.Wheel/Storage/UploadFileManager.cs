using System.Net;
using Microsoft.AspNetCore.Http;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Extensions;
using pandx.Wheel.Folders;

namespace pandx.Wheel.Storage;

public class UploadFileManager : IUploadFileManager
{
    private const long MaxFileSize = 1024 * 1024 * 10;
    private readonly ICachedFileManager _cachedFileManager;
    private readonly ICommonFolder _commonFolder;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UploadFileManager(ICommonFolder commonFolder, ICachedFileManager cachedFileManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _commonFolder = commonFolder;
        _cachedFileManager = cachedFileManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual Task<List<UploadFile>> UploadLargeFilesToDiskAsync()
    {
        throw new NotImplementedException();
    }

    public virtual async Task<UploadFile> UploadLargeFileToDiskAsync()
    {
        var contentDisposition = _httpContextAccessor.HttpContext!.Request.Headers["Content-Disposition"];
        var contentRange = _httpContextAccessor.HttpContext!.Request.Headers["Content-Range"];
        var file = _httpContextAccessor.HttpContext!.Request.Form.Files.First();
        using (var stream = file.OpenReadStream())
        {
            var token = WebUtility.UrlDecode(contentDisposition.ToString().Split(';')[2].Split('=')[1]
                .Replace("\"", ""));
            var temp = Path.Combine(_commonFolder.TempFolder, token);
            using (var fileStream = new FileStream(temp, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                var buffer = new byte[1024];
                var length = await stream.ReadAsync(buffer, 0, 1024);
                while (length > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, length);
                    length = await stream.ReadAsync(buffer, 0, 1024);
                }

                await fileStream.FlushAsync();
            }

            if (!string.IsNullOrWhiteSpace(contentDisposition))
            {
                var allSize = Convert.ToInt64(contentRange[0]!.Split('/')[1]);
                var currentSize = Convert.ToInt64(contentRange[0]!.Split('/')[0].Split('-')[1]);
                if (currentSize == allSize)
                {
                    var savedName = Guid.NewGuid().ToString();
                    var originalName = WebUtility.UrlDecode(contentDisposition.ToString().Split(';')[1].Split('=')[1]
                        .Replace("\"", ""));
                    File.Move(temp,
                        Path.Combine(_commonFolder.LargeFilesFolder, savedName + Path.GetExtension(originalName)));
                    return new UploadFile
                    {
                        OriginalName = originalName,
                        SavedName = savedName + Path.GetExtension(originalName),
                        FileSize = allSize,
                        ContentType = file.ContentType
                    };
                }
            }
        }

        throw new WheelException("文件上传失败");
    }

    public virtual async Task<List<UploadFile>> UploadFilesToDiskAsync()
    {
        var results = new List<UploadFile>();
        var files = _httpContextAccessor.HttpContext!.Request.Form.Files;
        foreach (var file in files)
        {
            if (file.Length > MaxFileSize)
            {
                results.Add(new UploadFile
                {
                    OriginalName = file.FileName,
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    IsSuccess = false
                });
                continue;
            }

            byte[] bytes;
            using (var stream = file.OpenReadStream())
            {
                bytes = await stream.GetAllBytesAsync();
            }

            var savedName = Guid.NewGuid().ToString();
            await File.WriteAllBytesAsync(
                Path.Combine(_commonFolder.FilesFolder, savedName + Path.GetExtension(file.FileName)), bytes);
            results.Add(new UploadFile
            {
                OriginalName = file.FileName,
                SavedName = savedName + Path.GetExtension(file.FileName),
                FileSize = file.Length,
                ContentType = file.ContentType,
                IsSuccess = true
            });
        }

        return results;
    }

    public virtual async Task<UploadFile> UploadFileToDiskAsync()
    {
        var file = _httpContextAccessor.HttpContext!.Request.Form.Files.FirstOrDefault();

        if (file is null)
        {
            throw new WheelException("上传的文件不存在");
        }

        if (file.Length > MaxFileSize)
        {
            throw new WheelException($"文件超出 {MaxFileSize / (1012 * 1024)}M 大小的限制");
        }

        byte[] bytes;
        using (var stream = file.OpenReadStream())
        {
            bytes = await stream.GetAllBytesAsync();
        }

        var savedName = Guid.NewGuid().ToString();
        await File.WriteAllBytesAsync(
            Path.Combine(_commonFolder.FilesFolder, savedName + Path.GetExtension(file.FileName)), bytes);

        return new UploadFile
        {
            OriginalName = file.FileName,
            SavedName = savedName + Path.GetExtension(file.FileName),
            FileSize = file.Length,
            ContentType = file.ContentType,
            IsSuccess = true
        };
    }

    public virtual async Task<CachedFile> UploadFileToCacheAsync()
    {
        var file = _httpContextAccessor.HttpContext!.Request.Form.Files.FirstOrDefault();
        if (file is null)
        {
            throw new WheelException("上传的文件不存在");
        }

        if (file.Length > MaxFileSize)
        {
            throw new WheelException($"文件超出 {MaxFileSize / (1012 * 1024)}M 大小的限制");
        }

        byte[] bytes;
        using (var stream = file.OpenReadStream())
        {
            bytes = await stream.GetAllBytesAsync();
        }

        var token = Guid.NewGuid().ToString();
        await _cachedFileManager.SetFileAsync(token, file.FileName, bytes);
        return new CachedFile
        {
            Name = file.FileName,
            Token = token,
            Type = file.ContentType
        };
    }
}