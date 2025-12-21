using HitsterCardGenerator.Services;
using HitsterCardGenerator.Endpoints;
using QuestPDF.Infrastructure;

// Set QuestPDF community license
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Register services for dependency injection
builder.Services.AddSingleton<GenreValidator>();
builder.Services.AddSingleton<CsvParser>();

// Add OpenAPI services
builder.Services.AddOpenApi();

var app = builder.Build();

// Serve static files from wwwroot (Svelte build output)
app.UseDefaultFiles();
app.UseStaticFiles();

// Map OpenAPI endpoint
app.MapOpenApi();

// Map health check endpoints
app.MapHealthEndpoints();

// Map CSV upload endpoints
app.MapCsvEndpoints();

// Map match endpoints
app.MapMatchEndpoints();

// Map export endpoints
app.MapExportEndpoints();

// SPA fallback - serve index.html for client-side routing
app.MapFallbackToFile("index.html");

// Configure to listen on port 5000
app.Urls.Add("http://localhost:5000");

app.Run();

// Extension methods for endpoint mapping (defined in Endpoints/ folder)
public static partial class EndpointExtensions { }
