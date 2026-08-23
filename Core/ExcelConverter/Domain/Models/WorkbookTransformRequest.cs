namespace DocsData.ExcelConverter.Models;

// A sheet with no matching entry here passes through Transform unchanged.
public sealed record WorkbookTransformRequest(IReadOnlyList<SheetTransformRequest> Sheets);
