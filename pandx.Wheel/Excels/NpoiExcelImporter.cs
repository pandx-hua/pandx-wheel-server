using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace pandx.Wheel.Excels;

public abstract class NpoiExcelImporter<TEntity>
{
    protected List<TEntity> ProcessExcelFile(byte[] fileBytes, Func<ISheet, int, TEntity> processExcelRow)
    {
        var entities = new List<TEntity>();
        using var stream = new MemoryStream(fileBytes);
        var workbook = new XSSFWorkbook(stream);
        for (var i = 0; i < workbook.NumberOfSheets; i++)
        {
            var entitiesInWorksheet = ProcessWorksheet(workbook.GetSheetAt(i), processExcelRow);
            entities.AddRange(entitiesInWorksheet);
        }

        return entities;
    }

    private List<TEntity> ProcessWorksheet(ISheet sheet, Func<ISheet, int, TEntity> processExcelRow)
    {
        var entities = new List<TEntity>();
        var rowEnumerator = sheet.GetRowEnumerator();
        rowEnumerator.Reset();
        var i = 0;
        while (rowEnumerator.MoveNext())
        {
            if (i == 0)
            {
                //skip header
                i++;
                continue;
            }

            try
            {
                var entity = processExcelRow(sheet, i++);
                if (entity is not null)
                {
                    entities.Add(entity);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        return entities;
    }
}