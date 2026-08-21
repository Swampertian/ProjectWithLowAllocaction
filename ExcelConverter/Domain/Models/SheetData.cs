namespace DocsData.ExcelConverter.Models;

public sealed record SheetData(string Name, int RowCount, int ColumnCount, IReadOnlyList<string?[]> Rows);
