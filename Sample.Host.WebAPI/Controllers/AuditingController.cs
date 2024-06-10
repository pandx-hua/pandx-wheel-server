using Microsoft.AspNetCore.Mvc;
using pandx.Wheel.Authorization.Permissions;
using pandx.Wheel.Controllers;
using pandx.Wheel.Models;
using pandx.Wheel.Storage;
using Sample.Application.Auditing;
using Sample.Application.Auditing.Dto;
using Sample.Domain.Authorization.Permissions;

namespace Sample.Host.WebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuditingController : WheelControllerBase
{
    private readonly IAuditingAppService _auditingAppService;

    public AuditingController(IAuditingAppService auditingAppService)
    {
        _auditingAppService = auditingAppService;
    }

    [HttpPost(Name = nameof(GetPagedAuditing))]
    [NeedPermission(SamplePermissions.Resources.Auditing, SamplePermissions.Actions.Search)]
    public async Task<PagedResponse<AuditingDto>> GetPagedAuditing(GetAuditingRequest request)
    {
        return await _auditingAppService.GetPagedAuditingAsync(request);
    }

    [HttpPost(Name = nameof(GetAuditingToExcel))]
    [NeedPermission(SamplePermissions.Resources.Auditing, SamplePermissions.Actions.Export)]
    public async Task<CachedFile> GetAuditingToExcel(GetAuditingRequest request)
    {
        return await _auditingAppService.GetAuditingToExcelAsync(request);
    }
}