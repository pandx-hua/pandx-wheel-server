using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using pandx.Wheel.Auditing;
using pandx.Wheel.BackgroundJobs;
using pandx.Wheel.Domain.Repositories;
using pandx.Wheel.Domain.UnitOfWork;
using Quartz;

namespace Sample.Application.Auditing;

[ExposedJob("删除过期审计日志")]
public class ExpiredAuditingDeleteJob : BackgroundJob
{
    private readonly IRepository<AuditingInfo, Guid> _auditingInfoRepository;
    private readonly TimeSpan _expiredTime;
    private readonly ILogger<ExpiredAuditingDeleteJob> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ExpiredAuditingDeleteJob(
        IOptions<AuditingSettings> auditingSettings,
        IRepository<AuditingInfo, Guid> auditingInfoRepository,
        ILogger<ExpiredAuditingDeleteJob> logger,
        IUnitOfWork unitOfWork)

    {
        var settings = auditingSettings.Value;
        _expiredTime = TimeSpan.FromDays(settings.ExpiredAfterDays);
        _auditingInfoRepository = auditingInfoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public override BackgroundJobData BackgroundJobData { get; set; } = default!;

    public override Task Execute(IJobExecutionContext context)
    {
        var expiredTime = DateTime.Now - _expiredTime;
        return DeleteExpiredAuditingAsync(expiredTime);
    }

    private async Task DeleteExpiredAuditingAsync(DateTime expiredTime)
    {
        var expiredEntryCount = await _auditingInfoRepository.LongCountAsync(a => a.ExecutionTime < expiredTime);
        if (expiredEntryCount == 0)
        {
            return;
        }

        var expiredEntries = await _auditingInfoRepository.GetAllAsync(a => a.ExecutionTime < expiredTime);
        await expiredEntries.ExecuteDeleteAsync();
        await _unitOfWork.CommitAsync();
    }
}