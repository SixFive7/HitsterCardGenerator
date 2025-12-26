using HitsterCardGenerator.Services;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.Endpoints;

/// <summary>
/// Card preview image generation endpoints
/// </summary>
public static class CardPreviewEndpoints
{
    public static void MapCardPreviewEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/card-preview");

        // Generate front card preview (QR code)
        group.MapPost("/front", (CardPreviewRequest request, CardPreviewCache cache) =>
        {
            try
            {
                // Generate cache key
                var cacheKey = CardPreviewCache.FrontCardKey(request.TrackId, request.BackgroundColor);

                // Get from cache or generate
                var imageBytes = cache.GetOrCreate(cacheKey, () =>
                {
                    // Generate QR code for the track
                    var qrCode = QrCodeService.GenerateQrCode(request.TrackId);

                    // Create card data
                    var cardData = new CardData
                    {
                        Title = request.Title,
                        Artist = request.Artist,
                        Year = request.Year,
                        Genre = request.Genre,
                        QrCodeData = qrCode,
                        BackgroundColor = request.BackgroundColor
                    };

                    // Generate front card image using QuestPDF
                    return CardDesigner.GenerateFrontCardImage(cardData);
                });

                // Return PNG with cache headers
                return Results.File(
                    imageBytes,
                    contentType: "image/png",
                    enableRangeProcessing: false
                );
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: 500,
                    title: "Error generating front card preview"
                );
            }
        })
        .WithName("CardPreviewFront")
        .Produces(200, contentType: "image/png")
        .Produces(500);

        // Generate back card preview (year, artist, title, genre)
        group.MapPost("/back", (CardPreviewRequest request, CardPreviewCache cache) =>
        {
            try
            {
                // Generate cache key
                var cacheKey = CardPreviewCache.BackCardKey(request.TrackId, request.Year, request.BackgroundColor);

                // Get from cache or generate
                var imageBytes = cache.GetOrCreate(cacheKey, () =>
                {
                    // Create card data (no QR code needed for back)
                    var cardData = new CardData
                    {
                        Title = request.Title,
                        Artist = request.Artist,
                        Year = request.Year,
                        Genre = request.Genre,
                        BackgroundColor = request.BackgroundColor
                    };

                    // Generate back card image using QuestPDF
                    return CardDesigner.GenerateBackCardImage(cardData);
                });

                // Return PNG with cache headers
                return Results.File(
                    imageBytes,
                    contentType: "image/png",
                    enableRangeProcessing: false
                );
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: 500,
                    title: "Error generating back card preview"
                );
            }
        })
        .WithName("CardPreviewBack")
        .Produces(200, contentType: "image/png")
        .Produces(500);
    }
}

/// <summary>
/// Request model for card preview endpoints
/// </summary>
public record CardPreviewRequest
{
    /// <summary>Spotify track ID for QR code generation</summary>
    public string TrackId { get; init; } = string.Empty;

    /// <summary>Song title</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Artist name</summary>
    public string Artist { get; init; } = string.Empty;

    /// <summary>Release year</summary>
    public int Year { get; init; }

    /// <summary>Music genre</summary>
    public string Genre { get; init; } = string.Empty;

    /// <summary>Background color in hex format (e.g., "#FF6B6B")</summary>
    public string? BackgroundColor { get; init; }
}
