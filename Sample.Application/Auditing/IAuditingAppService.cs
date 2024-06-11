using pandx.Wheel.Application.Services;
using pandx.Wheel.Models;
using pandx.Wheel.Storage;
using Sample.Application.Auditing.Dto;

namespace Sample.Application.Auditing;

public interface IAuditingAppService : IApplicationService
{
    Task<PagedResponse<AuditingDto>> GetPagedAuditingAsync(GetAuditingRequest request);
    Task<CachedFile> GetAuditingToExcelAsync(GetAuditingRequest request);
}