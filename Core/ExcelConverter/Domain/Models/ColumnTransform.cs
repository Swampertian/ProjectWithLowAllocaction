namespace DocsData.ExcelConverter.Models;

// Desired final name/type for one column, index-aligned to the source SheetData.ColumnNames.
// Type must be one of the types ParquetWorkbookExporter understands (double, DateTime, bool,
// string) since those are the only ones the Export stage writes natively.
public sealed record ColumnTransform(string Name, Type Type);
