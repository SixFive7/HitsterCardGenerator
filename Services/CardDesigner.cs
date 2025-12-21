using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.Services;

/// <summary>
/// Service for designing front and back card layouts using QuestPDF
/// </summary>
public static class CardDesigner
{
    private const float CardWidth = 85f;  // mm
    private const float CardHeight = 55f; // mm
    private const float QrSize = 45f;     // mm

    /// <summary>
    /// Designs the front of a card (QR code centered)
    /// </summary>
    public static IDocument DesignFrontCard(CardData card)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(CardWidth, CardHeight, Unit.Millimetre);
                page.Margin(0);

                page.Content()
                    .Background(card.BackgroundColor ?? Colors.White)
                    .AlignCenter()
                    .AlignMiddle()
                    .Width(QrSize, Unit.Millimetre)
                    .Height(QrSize, Unit.Millimetre)
                    .Image(card.QrCodeData ?? Array.Empty<byte>())
                    .FitArea();
            });
        });
    }

    /// <summary>
    /// Designs the back of a card (artist, year, title, genre)
    /// </summary>
    public static IDocument DesignBackCard(CardData card)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(CardWidth, CardHeight, Unit.Millimetre);
                page.Margin(3, Unit.Millimetre);

                page.Content()
                    .Background(card.BackgroundColor ?? Colors.White)
                    .Padding(2, Unit.Millimetre)
                    .Column(col =>
                    {
                        col.Spacing(2);

                        // Artist (top, smaller)
                        col.Item()
                            .AlignCenter()
                            .Text(card.Artist)
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken2);

                        // Year (center, large and bold)
                        col.Item()
                            .AlignCenter()
                            .PaddingVertical(4, Unit.Millimetre)
                            .Text(card.Year.ToString())
                            .FontSize(28)
                            .Bold()
                            .FontColor(Colors.Black);

                        // Title (below year, medium)
                        col.Item()
                            .AlignCenter()
                            .Text(card.Title)
                            .FontSize(11)
                            .FontColor(Colors.Grey.Darken3);

                        // Spacer to push genre to bottom
                        col.Item().ExtendVertical();

                        // Genre (bottom, small)
                        col.Item()
                            .AlignCenter()
                            .Text(card.Genre)
                            .FontSize(8)
                            .FontColor(Colors.Grey.Medium);
                    });
            });
        });
    }

    /// <summary>
    /// Generates the front card as a PNG byte array
    /// </summary>
    public static byte[] GenerateFrontCardImage(CardData card)
    {
        return DesignFrontCard(card).GenerateImages().First();
    }

    /// <summary>
    /// Generates the back card as a PNG byte array
    /// </summary>
    public static byte[] GenerateBackCardImage(CardData card)
    {
        return DesignBackCard(card).GenerateImages().First();
    }
}
