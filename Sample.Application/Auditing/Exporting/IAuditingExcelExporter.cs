using pandx.Wheel.DependencyInjection;
using pandx.Wheel.Storage;
using Sample.Application.Auditing.Dto;

namespace Sample.Application.Auditing.Exporting;

public interface IAuditingExcelExporter : ITransientDependency
{
    Task<CachedFile> ExportToExcelAsync(List<AuditingDto> dtos);
}