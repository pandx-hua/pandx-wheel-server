using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Auditing;
using pandx.Wheel.Controllers;
using pandx.Wheel.Storage;
using Sample.Application.Logging;
using Sample.Application.Logging.Dto;

namespace Sample.Host.WebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class LoggingController : WheelControllerBase
{
    private readonly ICachedFileManager _cachedFileManager;
    private readonly ILogAppService _logAppService;

    public LoggingController(ILogAppService logAppService, ICachedFileManager cachedFileManager)
    {
        _logAppService = logAppService;
        _cachedFileManager = cachedFileManager;
    }

    [HttpPost(Name = nameof(DownloadLogs))]
    public async Task<CachedFile> DownloadLogs()
    {
        return await _logAppService.DownloadLogsAsync();
    }

    [HttpPost(Name = nameof(GetLatestLogs))]
    [NoAudited]
    public async Task<GetLatestLogsResponse> GetLatestLogs()
    {
        return await _logAppService.GetLatestLogsAsync();
    }
}