using DocsData.ExcelConverter.Models;

namespace DocsData.ExcelConverter.Abstractions;

public interface ISchemaDiagramGenerator
{
    SchemaGraph Generate(WorkbookData workbook);
}
