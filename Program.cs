using System.Text;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DocsData;
using DocsData.ExcelConverter.Abstractions;
using DocsData.ExcelConverter.Ingestion;
using DocsData.ExcelConverter.Parsing;
using DocsData.ExcelConverter.Pipeline;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IExcelFileValidator, ExcelFileValidator>();
builder.Services.AddScoped<IFileIngestor, BrowserFileIngestor>();
builder.Services.AddScoped<ICellNormalizer, InvariantCellNormalizer>();
builder.Services.AddScoped<IWorkbookParser, ExcelWorkbookParser>();
builder.Services.AddScoped<IExcelPipeline, ExcelPipeline>();

await builder.Build().RunAsync();
