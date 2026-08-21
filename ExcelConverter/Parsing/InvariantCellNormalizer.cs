using System.Globalization;
using DocsData.ExcelConverter.Abstractions;

namespace DocsData.ExcelConverter.Parsing;

public sealed class InvariantCellNormalizer : ICellNormalizer
{
    public string? Normalize(object? value) => value switch
    {
        null => null,
        string s => s.Trim(),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString()
    };
}
