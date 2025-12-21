using HitsterCardGenerator.Services;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.Endpoints;

/// <summary>
/// CSV file upload and parsing endpoints
/// </summary>
public static class CsvEndpoints
{
    public static void MapCsvEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/csv");

        // Upload and parse CSV file
        group.MapPost("/upload", async (HttpRequest request, CsvParser csvParser) =>
        {
            try
            {
                // Check if request contains form data
                if (!request.HasFormContentType)
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        errorSummary = "Request must be multipart/form-data"
                    });
                }

                var form = await request.ReadFormAsync();
                var file = form.Files.GetFile("file");

                if (file == null || file.Length == 0)
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        errorSummary = "No file provided"
                    });
                }

                // Check file extension
                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest(new
                    {
                        success = false,
                        errorSummary = "File must be a CSV file"
                    });
                }

                // Save file temporarily
                var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");

                try
                {
                    // Write uploaded file to temp location
                    using (var stream = File.Create(tempPath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Parse the CSV file
                    var result = csvParser.ParseFile(tempPath);

                    // Return structured response
                    return Results.Ok(new
                    {
                        success = true,
                        totalSongs = result.Songs.Count,
                        validSongs = result.ValidSongs,
                        invalidSongs = result.InvalidSongs,
                        errorSummary = result.ErrorSummary
                    });
                }
                finally
                {
                    // Clean up temp file
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: 500,
                    title: "Error processing CSV file"
                );
            }
        })
        .WithName("UploadCsv")
        .DisableAntiforgery(); // Allow file uploads without antiforgery token
    }
}
