using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Auditing;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Controllers;
using pandx.Wheel.Storage;
using Sample.Application.Logging;
using Sample.Application.Logging.Dto;
using Sample.Domain.Authorization.Permissions;

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

    [NeedPermission(SamplePermissions.Resources.Logs, SamplePermissions.Actions.Export)]
    [HttpPost(Name = nameof(DownloadLogs))]
    public async Task<CachedFile> DownloadLogs()
    {
        return await _logAppService.DownloadLogsAsync();
    }

    [NeedPermission(SamplePermissions.Resources.Logs, SamplePermissions.Actions.Search)]
    [HttpPost(Name = nameof(GetLatestLogs))]
    [NoAudited]
    public async Task<GetLatestLogsResponse> GetLatestLogs()
    {
        return await _logAppService.GetLatestLogsAsync();
    }
}