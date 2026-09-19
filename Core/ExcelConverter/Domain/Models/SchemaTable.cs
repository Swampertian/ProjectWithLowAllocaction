namespace DocsData.ExcelConverter.Models;

public sealed record SchemaTable(string Name, IReadOnlyList<SchemaColumn> Columns);
