namespace DocsData.ExcelConverter.Models;

// Columns must be index-aligned to the target sheet's SheetData.ColumnNames/ColumnTypes.
public sealed record SheetTransformRequest(string SheetName, IReadOnlyList<ColumnTransform> Columns);
