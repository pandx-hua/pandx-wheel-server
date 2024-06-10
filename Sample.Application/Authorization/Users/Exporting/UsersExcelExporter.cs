using pandx.Wheel.Authorization.Users;
using pandx.Wheel.Excels;
using pandx.Wheel.Storage;
using Sample.Application.Authorization.Users.Dto;

namespace Sample.Application.Authorization.Users.Exporting;

public class UsersExcelExporter : NpoiExcelExporter, IUsersExcelExporter
{
    public UsersExcelExporter(ICachedFileManager cachedFileManager) : base(cachedFileManager)
    {
    }

    public Task<CachedFile> ExportToExcelAsync(List<UsersDto> dtos)
    {
        return CreateExcelPackageAsync($"Users-{DateTime.Now:yyyyMMddHHmmss}.xlsx", excelPackage =>
        {
            var sheet = excelPackage.CreateSheet("UsersList");
            AddHeader(sheet,
                "所属部门",
                "账号",
                "姓名",
                "性别",
                "所属角色",
                "激活状态",
                "锁定状态",
                "微信绑定",
                "Email地址",
                "电话号码",
                "创建时间"
            );
            AddObjects(sheet, dtos,
                o => string.Join('，', o.Organizations.Select(uo => uo.DisplayName)),
                o => o.UserName,
                o => o.Name,
                o => o.Gender == Gender.Male ? "男" : "女",
                o => string.Join('，', o.Roles.Select(r => r.DisplayName)),
                o => o.IsActive ? "是" : "否",
                o => o.IsLockout ? "是" : "否",
                o => o.IsWeixin ? "是" : "否",
                o => o.Email,
                o => o.PhoneNumber,
                o => o.CreationTime
            );
            for (var i = 1; i < dtos.Count; i++)
            {
                SetCellDataFormat(sheet.GetRow(i).Cells[10], "yyyy-MM-dd HH:mm:ss");
            }

            for (var i = 0; i < sheet.GetRow(0).Cells.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        });
    }
}