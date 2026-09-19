namespace DocsData.ExcelConverter.Models;

// Display-only column entry for the schema preview. TypeName is a short label (string/double/
// datetime/bool), not the CLR Type, since the UI's live preview builds this from user-edited
// state that only ever has a type name, never a CLR Type.
public sealed record SchemaColumn(string Name, string TypeName);
