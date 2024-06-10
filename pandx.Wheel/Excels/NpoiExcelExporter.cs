using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using pandx.Wheel.Extensions;
using pandx.Wheel.Storage;

namespace pandx.Wheel.Excels;

public abstract class NpoiExcelExporter
{
    private readonly ICachedFileManager _cachedFileManager;

    protected NpoiExcelExporter(ICachedFileManager cachedFileManager)
    {
        _cachedFileManager = cachedFileManager;
    }

    protected virtual async Task<CachedFile> CreateExcelPackageAsync(string fileName, Action<XSSFWorkbook> creator)
    {
        var cachedFile = new CachedFile(fileName,
            MimeTypes.MimeTypes.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
        var workbook = new XSSFWorkbook();
        creator(workbook);
        await SaveAsync(workbook, cachedFile);
        return cachedFile;
    }

    protected void AddHeader(ISheet sheet, params string[] headerTexts)
    {
        if (headerTexts.IsNullOrEmpty())
        {
            return;
        }

        sheet.CreateRow(0);
        for (var i = 0; i < headerTexts.Length; i++)
        {
            AddHeader(sheet, i, headerTexts[i]);
        }
    }

    private void AddHeader(ISheet sheet, int columnIndex, string headerText)
    {
        var cell = sheet.GetRow(0).CreateCell(columnIndex);
        cell.SetCellValue(headerText);
        var cellStyle = sheet.Workbook.CreateCellStyle();
        var font = sheet.Workbook.CreateFont();
        font.IsBold = true;
        font.FontHeightInPoints = 14;
        cellStyle.SetFont(font);
        cell.CellStyle = cellStyle;
    }

    protected void AddObjects<T>(ISheet sheet, IList<T> items,
        params Func<T, object?>[] propertySelectors)
    {
        if (items.IsNullOrEmpty() || propertySelectors.IsNullOrEmpty())
        {
            return;
        }

        for (var i = 0; i < items.Count; i++)
        {
            var row = sheet.CreateRow(i + 1);
            for (var j = 0; j < propertySelectors.Length; j++)
            {
                var cell = row.CreateCell(j);
                var value = propertySelectors[j](items[i]);
                if (value is not null)
                {
                    cell.SetCellValue(value.ToString());
                }
            }
        }
    }

    private async Task SaveAsync(XSSFWorkbook excelPackage, CachedFile cachedFile)
    {
        using var stream = new MemoryStream();
        excelPackage.Write(stream);
        await _cachedFileManager.SetFileAsync(cachedFile.Token, cachedFile.Name, stream.ToArray());
    }

    protected void SetCellDataFormat(ICell? cell, string format)
    {
        if (cell is null)
        {
            return;
        }

        var cellStyle = cell.Sheet.Workbook.CreateCellStyle();
        var dataFormat = cell.Sheet.Workbook.CreateDataFormat();
        cellStyle.DataFormat = dataFormat.GetFormat(format);
        cell.CellStyle = cellStyle;
        if (DateTime.TryParse(cell.StringCellValue, out var datetime))
        {
            cell.SetCellValue(datetime);
        }
    }
}