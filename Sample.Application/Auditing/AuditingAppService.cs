using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using pandx.Wheel.Auditing;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Extensions;
using pandx.Wheel.Models;
using pandx.Wheel.Storage;
using Sample.Application.Auditing.Dto;
using Sample.Application.Auditing.Exporting;
using Sample.Domain.Authorization.Users;

namespace Sample.Application.Auditing;

public class AuditingAppService : SampleAppServiceBase, IAuditingAppService
{
    private readonly IAuditingExcelExporter _auditingExcelExporter;
    private readonly IRepository<AuditingInfo, Guid> _auditingRepository;

    public AuditingAppService(IRepository<AuditingInfo, Guid> auditingRepository,
        IAuditingExcelExporter auditingExcelExporter)
    {
        _auditingRepository = auditingRepository;
        _auditingExcelExporter = auditingExcelExporter;
    }

    public async Task<PagedResponse<AuditingDto>> GetPagedAuditingAsync(GetAuditingRequest request)
    {
        var query = GetAuditingFilteredQuery(request);
        var totalCount = await query.CountAsync();
        var auditing = await query.OrderBy(request.Sorting!).PageBy(request).ToListAsync();
        var dtos = auditing.Select(item =>
        {
            var dto = Mapper.Map<AuditingDto>(item.Auditing);
            dto.UserName = item.User?.UserName;
            dto.HasException = !string.IsNullOrWhiteSpace(item.Auditing.Exception);
            return dto;
        }).ToList();

        return new PagedResponse<AuditingDto>(totalCount, dtos);
    }

    public async Task<CachedFile> GetAuditingToExcelAsync(GetAuditingRequest request)
    {
        var auditing = await GetAuditingFilteredQuery(request)
            .AsNoTracking()
            .OrderByDescending(x => x.Auditing.ExecutionTime)
            .ToListAsync();
        var dtos = auditing.Select(item =>
        {
            var dto = Mapper.Map<AuditingDto>(item.Auditing);
            dto.UserName = item.User?.UserName;
            return dto;
        }).ToList();

        return await _auditingExcelExporter.ExportToExcelAsync(dtos);
    }

    private IQueryable<AuditingAndUser> GetAuditingFilteredQuery(GetAuditingRequest request)
    {
        var query = from auditing in _auditingRepository.GetAll()
            join user in UserService.UserManager.Users on auditing.UserId equals user.Id into userJoin
            from joinedUser in userJoin.DefaultIfEmpty()
            where auditing.ExecutionTime >= request.StartTime && auditing.ExecutionTime <= request.EndTime
            select new AuditingAndUser
            {
                Auditing = auditing,
                User = joinedUser
            };

        query = query
            .WhereIf(request.HasException.Count == 1 && request.HasException.Contains(true),
                x => !string.IsNullOrWhiteSpace(x.Auditing.Exception))
            .WhereIf(request.HasException.Count == 1 && request.HasException.Contains(false),
                x => string.IsNullOrWhiteSpace(x.Auditing.Exception))
            .WhereIf(!string.IsNullOrWhiteSpace(request.UserName),
                x => x.User != null && x.User.UserName!.Contains(request.UserName!))
            .WhereIf(!string.IsNullOrWhiteSpace(request.Controller),
                x => x.Auditing.Controller != null && x.Auditing.Controller.Contains(request.Controller!))
            .WhereIf(!string.IsNullOrWhiteSpace(request.Action),
                x => x.Auditing.Action != null && x.Auditing.Action.Contains(request.Action!))
            .WhereIf(!string.IsNullOrWhiteSpace(request.ClientIpAddress),
                x => x.Auditing.ClientIpAddress != null &&
                     x.Auditing.ClientIpAddress.Contains(request.ClientIpAddress!));

        return query;
    }

    private class AuditingAndUser
    {
        public AuditingInfo Auditing { get; set; } = default!;
        public ApplicationUser? User { get; set; }
    }
}