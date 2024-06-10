using pandx.Wheel.Excels;
using pandx.Wheel.Storage;
using Sample.Application.Authorization.Users.Importing.Dto;

namespace Sample.Application.Authorization.Users.Importing;

public class InvalidUsersExcelExporter : NpoiExcelExporter, IInvalidUsersExcelExporter
{
    public InvalidUsersExcelExporter(ICachedFileManager cachedFileManager) : base(cachedFileManager)
    {
    }

    public Task<CachedFile> ExportToExcelAsync(List<ImportedUserDto> dtos)
    {
        return CreateExcelPackageAsync($"InvalidUsers-{DateTime.Now:yyyyMMddHHmmss}.xlsx", excelPackage =>
        {
            var sheet = excelPackage.CreateSheet("InvalidUsersList");
            AddHeader(sheet,
                "账号",
                "姓名",
                "错误原因"
            );
            AddObjects(sheet, dtos,
                o => o.UserName,
                o => o.Name,
                o => o.Exception
            );
            // for (var i = 1; i < dtos.Count; i++)
            // {
            //     SetCellDataFormat(sheet.GetRow(i).Cells[8], "yyyy-MM-dd HH:mm:ss");
            // }

            for (var i = 0; i < sheet.GetRow(0).Cells.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        });
    }
}