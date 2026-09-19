using DocsData.ExcelConverter.Abstractions;
using DocsData.ExcelConverter.Models;

namespace DocsData.ExcelConverter.Preview;

// Builds the structural schema view for the interactive Cytoscape preview: one table per sheet,
// columns as-is. No foreign-key info -- Excel sheets don't encode relationships between sheets.
public sealed class WorkbookSchemaGraphGenerator : ISchemaDiagramGenerator
{
    public SchemaGraph Generate(WorkbookData workbook)
    {
        var tables = workbook.Sheets
            .Select(sheet => new SchemaTable(
                sheet.Name,
                sheet.ColumnNames
                    .Select((name, i) => new SchemaColumn(name, DisplayType(sheet.ColumnTypes[i])))
                    .ToList()))
            .ToList();

        return new SchemaGraph(tables);
    }

    private static string DisplayType(Type type) =>
        type == typeof(double) ? "double" :
        type == typeof(DateTime) ? "datetime" :
        type == typeof(bool) ? "bool" :
        "string";
}
