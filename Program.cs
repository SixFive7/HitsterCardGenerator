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

// Map OpenAPI endpoint
app.MapOpenApi();

// Map health check endpoints
app.MapHealthEndpoints();

// Map CSV upload endpoints
app.MapCsvEndpoints();

// Configure to listen on port 5000
app.Urls.Add("http://localhost:5000");

app.Run();

// Extension methods for endpoint mapping (defined in Endpoints/ folder)
public static partial class EndpointExtensions { }
