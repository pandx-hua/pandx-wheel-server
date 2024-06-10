using pandx.Wheel.Application.Services;
using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Storage;
using Sample.Application.Logging.Dto;

namespace Sample.Application.Logging;

public interface ILogAppService : IApplicationService
{
    Task<CachedFile> DownloadLogsAsync();
    Task<GetLatestLogsResponse> GetLatestLogsAsync();
}