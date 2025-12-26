using HitsterCardGenerator.Services;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.Endpoints;

/// <summary>
/// PDF export endpoints
/// </summary>
public static class ExportEndpoints
{
    public static void MapExportEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api");

        // Export cards to PDF
        group.MapPost("/export", (ExportRequest request) =>
        {
            try
            {
                // Validate request
                if (request.Cards == null || request.Cards.Count == 0)
                {
                    return Results.BadRequest("No cards provided for export");
                }

                // Generate cards with QR codes and colors
                var cards = new List<CardData>();
                foreach (var card in request.Cards)
                {
                    // Generate QR code for the track
                    var qrCode = QrCodeService.GenerateQrCode(card.TrackId);

                    // Get background color from genre colors dictionary
                    var backgroundColor = request.GenreColors.TryGetValue(card.Genre, out var color)
                        ? color
                        : null;

                    cards.Add(new CardData
                    {
                        Title = card.Title,
                        Artist = card.Artist,
                        Year = card.Year,
                        Genre = card.Genre,
                        QrCodeData = qrCode,
                        BackgroundColor = backgroundColor,
                        AlbumImageUrl = card.AlbumImageUrl,
                        AlbumName = card.AlbumName
                    });
                }

                // Generate smart filename: hitster-cards-{count}-{date}.pdf
                var filename = $"hitster-cards-{cards.Count}-{DateTime.Now:yyyy-MM-dd}.pdf";
                var tempPath = Path.Combine(Path.GetTempPath(), filename);

                // Generate PDF with cutting lines option
                var pageCount = PdfExporter.ExportToPdf(cards, tempPath, request.CuttingLines);

                // Read PDF bytes and clean up temp file
                var pdfBytes = File.ReadAllBytes(tempPath);
                File.Delete(tempPath);

                // Return PDF with download headers
                return Results.File(
                    pdfBytes,
                    contentType: "application/pdf",
                    fileDownloadName: filename
                );
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: 500,
                    title: "Error generating PDF"
                );
            }
        })
        .WithName("ExportPdf")
        .Produces(200, contentType: "application/pdf")
        .Produces(400)
        .Produces(500);
    }
}
