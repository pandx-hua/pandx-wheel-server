using pandx.Wheel.Excels;
using pandx.Wheel.Storage;
using Sample.Application.Auditing.Dto;

namespace Sample.Application.Auditing.Exporting;

public class AuditingExcelExporter : NpoiExcelExporter, IAuditingExcelExporter
{
    public AuditingExcelExporter(ICachedFileManager cachedFileManager) : base(cachedFileManager)
    {
    }

    public Task<CachedFile> ExportToExcelAsync(List<AuditingDto> dtos)
    {
        return CreateExcelPackageAsync($"Auditing-{DateTime.Now:yyyyMMddHHmmss}.xlsx", excelPackage =>
        {
            var sheet = excelPackage.CreateSheet("AuditingLogsList");
            AddHeader(sheet,
                "执行时间",
                "账号",
                "控制器",
                "方法",
                "参数",
                "持续时间",
                "客户端",
                "IP地址",
                "浏览器",
                "状态"
            );
            AddObjects(sheet, dtos,
                o => o.ExecutionTime,
                o => o.UserName,
                o => o.Controller,
                o => o.Action,
                o => o.Parameters,
                o => $"{o.ExecutionDuration}ms",
                o => o.ClientName,
                o => o.ClientIpAddress,
                o => o.BrowserInfo,
                o => string.IsNullOrWhiteSpace(o.Exception) ? "成功" : o.Exception
            );
            for (var i = 1; i < dtos.Count; i++)
            {
                SetCellDataFormat(sheet.GetRow(i).Cells[0], "yyyy-MM-dd HH:mm:ss");
            }

            for (var i = 0; i < sheet.GetRow(0).Cells.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        });
    }
}