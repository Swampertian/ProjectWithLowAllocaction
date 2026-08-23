using DocsData.ExcelConverter.Models;

namespace DocsData.ExcelConverter.Abstractions;

public interface ISchemaDiagramGenerator
{
    string Generate(WorkbookData workbook);
}
