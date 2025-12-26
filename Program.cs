using HitsterCardGenerator.Services;
using HitsterCardGenerator.Endpoints;
using HitsterCardGenerator.Data;
using HitsterCardGenerator.Repositories;
using QuestPDF.Infrastructure;

// Set QuestPDF community license
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Register services for dependency injection
builder.Services.AddSingleton<GenreValidator>();
builder.Services.AddSingleton<CsvParser>();

// Add LiteDB database
builder.Services.Configure<LiteDbOptions>(
    builder.Configuration.GetSection("LiteDbOptions"));
builder.Services.AddSingleton<ILiteDbContext, LiteDbContext>();
builder.Services.AddSingleton<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddSingleton<ITrackRepository, TrackRepository>();

// Add memory cache and card preview cache
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<CardPreviewCache>();

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

// Map search endpoints
app.MapSearchEndpoints();

// Map export endpoints
app.MapExportEndpoints();

// Map card preview endpoints
app.MapCardPreviewEndpoints();

// SPA fallback - serve index.html for client-side routing
app.MapFallbackToFile("index.html");

app.Run();

// Extension methods for endpoint mapping (defined in Endpoints/ folder)
public static partial class EndpointExtensions { }
