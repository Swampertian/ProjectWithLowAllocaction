namespace DocsData.ExcelConverter.Models;

// Structural view of a workbook's schema for the interactive Cytoscape preview: one table per
// sheet. Carries no foreign-key data -- Excel sheets don't encode relationships between sheets.
public sealed record SchemaGraph(IReadOnlyList<SchemaTable> Tables);
