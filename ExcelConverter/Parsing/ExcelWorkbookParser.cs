using Sylvan.Data.Excel;
using DocsData.ExcelConverter.Abstractions;
using DocsData.ExcelConverter.Models;

namespace DocsData.ExcelConverter.Parsing;

// Single reader instance, single pass over the source bytes: each cell is read and
// normalized to string exactly once, straight into the final row array. No Raw Sheet
// intermediate and no second full-sheet pass — those were doubling both I/O and allocations.
public sealed class ExcelWorkbookParser(ICellNormalizer cellNormalizer) : IWorkbookParser
{
    public WorkbookData Parse(byte[] content, string fileName)
    {
        var workbookType = ExcelDataReader.GetWorkbookType(fileName);

        using var stream = new MemoryStream(content);
        using var reader = ExcelDataReader.Create(stream, workbookType);

        var sheets = new List<SheetData>();
        do
        {
            sheets.Add(ParseSheet(reader));
        } while (reader.NextResult());

        return new WorkbookData(fileName, sheets);
    }

    private SheetData ParseSheet(ExcelDataReader reader)
    {
        var rows = new List<string?[]>();
        var columnCount = 0;

        while (reader.Read())
        {
            var fieldCount = reader.RowFieldCount;
            columnCount = Math.Max(columnCount, fieldCount);

            var row = new string?[fieldCount];
            for (var i = 0; i < fieldCount; i++)
            {
                row[i] = ReadCell(reader, i);
            }
            rows.Add(row);
        }

        for (var r = 0; r < rows.Count; r++)
        {
            if (rows[r].Length < columnCount)
            {
                var padded = new string?[columnCount];
                Array.Copy(rows[r], padded, rows[r].Length);
                rows[r] = padded;
            }
        }

        return new SheetData(reader.WorksheetName ?? string.Empty, rows.Count, columnCount, rows);
    }

    private string? ReadCell(ExcelDataReader reader, int ordinal)
    {
        if (reader.GetExcelDataType(ordinal) == ExcelDataType.Error)
        {
            return $"#{reader.GetFormulaError(ordinal)}";
        }

        return reader.IsDBNull(ordinal) ? null : cellNormalizer.Normalize(reader.GetValue(ordinal));
    }
}
