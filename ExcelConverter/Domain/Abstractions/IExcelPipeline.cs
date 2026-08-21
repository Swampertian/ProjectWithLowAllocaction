using Microsoft.AspNetCore.Components.Forms;
using DocsData.ExcelConverter.Models;

namespace DocsData.ExcelConverter.Abstractions;

public interface IExcelPipeline
{
    Task<UploadResult> UploadAsync(IBrowserFile file, CancellationToken cancellationToken = default);

    Task<WorkbookData> ProcessAsync(byte[] content, string fileName, CancellationToken cancellationToken = default);
}
