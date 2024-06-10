using System.Text;
using NPOI.SS.UserModel;
using pandx.Wheel.Excels;
using Sample.Application.Authorization.Users.Importing.Dto;

namespace Sample.Application.Authorization.Users.Importing;

public class UsersExcelImporter : NpoiExcelImporter<ImportedUserDto>, IUsersExcelImporter
{
    public List<ImportedUserDto> GetUsersFromExcel(byte[] fileBytes)
    {
        return ProcessExcelFile(fileBytes, (sheet, row) =>
        {
            if (IsEmptyRow(sheet, row))
            {
                return null!;
            }

            var exceptionMessage = new StringBuilder();
            var user = new ImportedUserDto();

            try
            {
                user.UserName = GetRequiredValueFromRowOrNull(sheet, row, 0, "账号", exceptionMessage, CellType.String);
                user.Name = GetRequiredValueFromRowOrNull(sheet, row, 1, "姓名", exceptionMessage, CellType.String);
                user.GenderName = GetRequiredValueFromRowOrNull(sheet, row, 2, "性别", exceptionMessage, CellType.String);
                user.Email =
                    GetRequiredValueFromRowOrNull(sheet, row, 3, "Email地址", exceptionMessage, CellType.String);
                user.ActiveName =
                    GetRequiredValueFromRowOrNull(sheet, row, 4, "激活状态", exceptionMessage, CellType.String);
                user.PhoneNumber = GetOptionalValueFromRowOrNull(sheet, row, 5, exceptionMessage, CellType.String);
                user.Password = GetRequiredValueFromRowOrNull(sheet, row, 6, "密码", exceptionMessage, CellType.String);
                user.AssignedRoles = GetAssignedRolesFromRow(sheet, row, 7, CellType.String);
                user.AssignedOrganizations = GetAssignedOrganizationsFromRow(sheet, row, 8, CellType.String);
            }
            catch (Exception exception)
            {
                user.Exception = exception.Message + exception.StackTrace;
            }

            return user;
        });
    }

    private string GetRequiredValueFromRowOrNull(ISheet sheet, int row, int column, string columnName,
        StringBuilder exceptionMessage, CellType? cellType = null)
    {
        var cell = sheet.GetRow(row).GetCell(column);
        if (cellType.HasValue)
        {
            cell?.SetCellType(cellType.Value);
        }

        var cellValue = cell?.StringCellValue ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(cellValue))
        {
            return cellValue;
        }

        exceptionMessage.Append($"{columnName} 列内容未通过验证");
        return null!;
    }

    private string GetOptionalValueFromRowOrNull(ISheet sheet, int row, int column, StringBuilder exceptionMessage,
        CellType? cellType = null)
    {
        var cell = sheet.GetRow(row).GetCell(column);
        if (cellType.HasValue)
        {
            cell?.SetCellType(cellType.Value);
        }

        var cellValue = cell?.StringCellValue ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(cellValue))
        {
            return cellValue;
        }

        return string.Empty;
    }

    private string[] GetAssignedRolesFromRow(ISheet sheet, int row, int column, CellType? cellType = null)
    {
        var cell = sheet.GetRow(row).GetCell(column);
        if (cellType.HasValue)
        {
            cell?.SetCellType(cellType.Value);
        }

        var cellValue = cell?.StringCellValue ?? string.Empty;
        if (string.IsNullOrWhiteSpace(cellValue))
        {
            return Array.Empty<string>();
        }

        return cellValue.Split(",").Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim())
            .ToArray();
    }

    private string[] GetAssignedOrganizationsFromRow(ISheet sheet, int row, int column, CellType? cellType = null)
    {
        var cell = sheet.GetRow(row).GetCell(column);
        if (cellType.HasValue)
        {
            cell?.SetCellType(cellType.Value);
        }

        var cellValue = cell?.StringCellValue ?? string.Empty;
        if (string.IsNullOrWhiteSpace(cellValue))
        {
            return Array.Empty<string>();
        }

        return cellValue.Split(",").Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim())
            .ToArray();
    }


    private bool IsEmptyRow(ISheet sheet, int row)
    {
        var cell = sheet.GetRow(row)?.Cells.FirstOrDefault();
        return cell == null || string.IsNullOrWhiteSpace(cell.StringCellValue);
    }
}