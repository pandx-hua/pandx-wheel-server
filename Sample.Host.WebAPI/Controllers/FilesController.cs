using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Auditing;
using pandx.Wheel.Controllers;
using pandx.Wheel.Exceptions;
using pandx.Wheel.Folders;
using pandx.Wheel.MimeTypes;
using pandx.Wheel.Storage;
using Sample.Application.Files.Dto;

namespace Sample.Host.WebAPI.Controllers;
[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public class FilesController : WheelControllerBase
{
    private readonly ICachedFileManager _cachedFileManager;
    private readonly ICommonFolder _commonFolder;
    private readonly IMimeTypeManager _mimeTypeManager;
    private readonly IBinaryObjectManager _binaryObjectManager;

    public FilesController(ICachedFileManager cachedFileManager,
        IBinaryObjectManager binaryObjectManager,
        ICommonFolder commonFolder,
        IMimeTypeManager mimeTypeManager)
    {
        _cachedFileManager = cachedFileManager;
        _commonFolder = commonFolder;
        _mimeTypeManager = mimeTypeManager;
        _binaryObjectManager = binaryObjectManager;
    }

    [HttpPost(Name = nameof(DownloadFileFromCache))]
    [NoAudited]
    public async Task<IActionResult> DownloadFileFromCache(CachedFile file)
    {
        var bytes = await _cachedFileManager.GetFileAsync(file.Token, file.Name) ??
                    throw new Exception("未在缓存中找到请求的文件");
        return File(bytes, file.Type, file.Name);
    }

    [HttpPost(Name = nameof(DownloadFileFromDisk1))]
    [NoAudited]
    public IActionResult DownloadFileFromDisk1(DownloadRequest request)
    {
        return File(
            new FileStream(Path.Combine(_commonFolder.FilesFolder, $"{request.Token}-{request.Name}"), FileMode.Open),
            _mimeTypeManager.GetMimeType(request.Name!) ?? "application/octet-stream",
            $"{request.Token}{request.Name}");
    }

    [HttpPost(Name = nameof(DownloadFileFromDisk2))]
    [NoAudited]
    public IActionResult DownloadFileFromDisk2(DownloadRequest request)
    {
        return File(new FileStream(Path.Combine(_commonFolder.FilesFolder, request.FileName!), FileMode.Open),
            _mimeTypeManager.GetMimeType(request.FileName!) ?? "application/octet-stream",
            request.FileName);
    }
    [HttpPost(Name = nameof(DownloadLargeFileFromDisk))]
    [NoAudited]
    public IActionResult DownloadLargeFileFromDisk(DownloadRequest request)
    {
        return File(new FileStream(Path.Combine(_commonFolder.LargeFilesFolder, request.FileName!), FileMode.Open),
            _mimeTypeManager.GetMimeType(request.FileName!) ?? "application/octet-stream",
            request.FileName);
    }

    [HttpPost(Name = nameof(DownloadBinaryFileFromDb))]
    [NoAudited]
    public async Task<IActionResult> DownloadBinaryFileFromDb(DownloadRequest request)
    {
        var binaryObject = await _binaryObjectManager.GetAsync(request.ObjectId!.Value) ??
                           throw new WheelException("没有找到指定的文件");
        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            if (!string.IsNullOrWhiteSpace(binaryObject.FileName))
            {
                request.FileName = binaryObject.FileName;
            }
            else if (!string.IsNullOrWhiteSpace(binaryObject.Description))
            {
                request.FileName = binaryObject.Description;
            }
            else
            {
                request.FileName = "未命名文件";
            }
        }

        if (string.IsNullOrWhiteSpace(request.ContentType))
        {
            if (!string.IsNullOrWhiteSpace(binaryObject.ContentType))
            {
                 request.ContentType = binaryObject.ContentType;
            }
            else
            {
                request.ContentType = _mimeTypeManager.GetMimeType(request.FileName!) ?? "application/octet-stream";
            }
        }

        return File(binaryObject.Bytes, request.ContentType, request.FileName);
    }
}