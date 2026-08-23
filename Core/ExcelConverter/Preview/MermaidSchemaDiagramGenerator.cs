using System.Text;
using DocsData.ExcelConverter.Abstractions;
using DocsData.ExcelConverter.Models;

namespace DocsData.ExcelConverter.Preview;

// Renders the exported Parquet schema (one table per sheet, columns = SheetData.ColumnNames/Types)
// as a Mermaid classDiagram. Excel sheets carry no foreign-key information, so tables are shown
// standalone with no inferred relationships.
public sealed class MermaidSchemaDiagramGenerator : ISchemaDiagramGenerator
{
    public string Generate(WorkbookData workbook)
    {
        var sb = new StringBuilder();
        sb.AppendLine("classDiagram");

        var usedAliases = new HashSet<string>(StringComparer.Ordinal);
        foreach (var sheet in workbook.Sheets)
        {
            var alias = UniqueAlias(sheet.Name, usedAliases);
            var label = EscapeLabel(sheet.Name);
            sb.AppendLine($"    class {alias}[\"{label}\"] {{");

            for (var i = 0; i < sheet.ColumnNames.Count; i++)
            {
                var typeName = MermaidType(sheet.ColumnTypes[i]);
                var fieldName = SanitizeIdentifier(sheet.ColumnNames[i]);
                sb.AppendLine($"        +{typeName} {fieldName}");
            }

            sb.AppendLine("    }");
        }

        return sb.ToString();
    }

    private static string MermaidType(Type type) =>
        type == typeof(double) ? "double" :
        type == typeof(DateTime) ? "datetime" :
        type == typeof(bool) ? "bool" :
        "string";

    private static string UniqueAlias(string sheetName, HashSet<string> usedAliases)
    {
        var sanitized = SanitizeIdentifier(sheetName);
        if (string.IsNullOrEmpty(sanitized) || !char.IsLetter(sanitized[0]))
        {
            sanitized = "T_" + sanitized;
        }

        var candidate = sanitized;
        var suffix = 1;
        while (!usedAliases.Add(candidate))
        {
            candidate = $"{sanitized}_{suffix++}";
        }
        return candidate;
    }

    private static string SanitizeIdentifier(string value)
    {
        var chars = value.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray();
        var result = new string(chars);
        return result.Length == 0 ? "_" : result;
    }

    private static string EscapeLabel(string value) => value.Replace("\"", "'");
}
