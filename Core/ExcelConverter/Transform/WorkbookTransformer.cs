using DocsData.ExcelConverter.Abstractions;
using DocsData.ExcelConverter.Models;

namespace DocsData.ExcelConverter.Transform;

// Applies user-chosen column renames/type overrides after Parsing and before Export. Parsing
// never sees these overrides — it only ever returns Sylvan's raw CLR values — so this is the one
// place a column's declared Type is made to match every row's actual runtime value, which is
// what ParquetWorkbookExporter's unboxing cast relies on.
public sealed class WorkbookTransformer : IWorkbookTransformer
{
    public WorkbookData Transform(WorkbookData workbook, WorkbookTransformRequest request)
    {
        var requestsByName = request.Sheets.ToDictionary(s => s.SheetName, StringComparer.Ordinal);

        var sheets = workbook.Sheets
            .Select(sheet => requestsByName.TryGetValue(sheet.Name, out var sheetRequest)
                ? TransformSheet(sheet, sheetRequest)
                : sheet)
            .ToList();

        return workbook with { Sheets = sheets };
    }

    private static SheetData TransformSheet(SheetData sheet, SheetTransformRequest request)
    {
        var columnCount = sheet.ColumnNames.Count;
        var columnNames = new string[columnCount];
        var columnTypes = new Type[columnCount];

        for (var i = 0; i < columnCount; i++)
        {
            var column = i < request.Columns.Count ? request.Columns[i] : null;
            columnNames[i] = column?.Name ?? sheet.ColumnNames[i];
            columnTypes[i] = column?.Type ?? sheet.ColumnTypes[i];
        }

        var rows = new List<object?[]>(sheet.Rows.Count);
        foreach (var sourceRow in sheet.Rows)
        {
            var row = new object?[columnCount];
            for (var i = 0; i < columnCount; i++)
            {
                row[i] = columnTypes[i] == sheet.ColumnTypes[i]
                    ? sourceRow[i]
                    : CellTypeConverter.Convert(sourceRow[i], columnTypes[i]);
            }
            rows.Add(row);
        }

        return sheet with { ColumnNames = columnNames, ColumnTypes = columnTypes, Rows = rows };
    }
}
