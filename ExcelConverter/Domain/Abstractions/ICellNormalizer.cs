namespace DocsData.ExcelConverter.Abstractions;

public interface ICellNormalizer
{
    string? Normalize(object? value);
}
