using System.Collections.Concurrent;
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
    private const float QrSize = 38f;     // mm (slightly smaller to fit genre text)
    private const float BarHeight = 8f;   // mm - height of text bars

    // Simple in-memory cache for album art during session
    private static readonly ConcurrentDictionary<string, byte[]?> AlbumImageCache = new();
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(10) };

    /// <summary>
    /// Determines if a hex color is dark (for choosing contrasting text color)
    /// </summary>
    private static bool IsDarkColor(string? hexColor)
    {
        if (string.IsNullOrWhiteSpace(hexColor))
            return false;

        try
        {
            var hex = hexColor.TrimStart('#');
            if (hex.Length != 6)
                return false;

            var r = Convert.ToInt32(hex.Substring(0, 2), 16);
            var g = Convert.ToInt32(hex.Substring(2, 2), 16);
            var b = Convert.ToInt32(hex.Substring(4, 2), 16);

            // Calculate luminance using standard formula
            var luminance = 0.299 * r + 0.587 * g + 0.114 * b;
            return luminance < 128;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Fetches album image from URL with caching
    /// </summary>
    private static byte[]? FetchAlbumImage(string? albumImageUrl)
    {
        if (string.IsNullOrWhiteSpace(albumImageUrl))
            return null;

        return AlbumImageCache.GetOrAdd(albumImageUrl, url =>
        {
            try
            {
                var response = HttpClient.GetAsync(url).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                }
            }
            catch
            {
                // Silently fail - album art is optional
            }
            return null;
        });
    }

    /// <summary>
    /// Designs the front of a card (QR code centered with genre text below)
    /// </summary>
    public static IDocument DesignFrontCard(CardData card)
    {
        var isDark = IsDarkColor(card.BackgroundColor);
        var textColor = isDark ? Colors.White : Colors.Grey.Darken4;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(CardWidth, CardHeight, Unit.Millimetre);
                page.Margin(0);

                page.Content()
                    .Background(card.BackgroundColor ?? Colors.White)
                    .Column(col =>
                    {
                        // Top spacer
                        col.Item().Height(4, Unit.Millimetre);

                        // QR code centered
                        col.Item()
                            .AlignCenter()
                            .Width(QrSize, Unit.Millimetre)
                            .Height(QrSize, Unit.Millimetre)
                            .Image(card.QrCodeData ?? Array.Empty<byte>())
                            .FitArea();

                        // Spacer between QR and text
                        col.Item().Height(2, Unit.Millimetre);

                        // Genre text centered below QR
                        col.Item()
                            .AlignCenter()
                            .Text(card.Genre)
                            .FontSize(10)
                            .Bold()
                            .FontColor(textColor);
                    });
            });
        });
    }

    /// <summary>
    /// Designs the back of a card (top bar with year/genre, album art center, bottom bar with artist/title/album)
    /// </summary>
    public static IDocument DesignBackCard(CardData card)
    {
        var albumImage = FetchAlbumImage(card.AlbumImageUrl);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(CardWidth, CardHeight, Unit.Millimetre);
                page.Margin(0);

                page.Content()
                    .Background(card.BackgroundColor ?? Colors.White)
                    .Column(col =>
                    {
                        // Top bar: Year | Genre
                        col.Item()
                            .Height(BarHeight, Unit.Millimetre)
                            .Background("#00000080") // Semi-transparent black
                            .AlignCenter()
                            .AlignMiddle()
                            .Text(text =>
                            {
                                text.Span(card.Year.ToString())
                                    .FontSize(9)
                                    .Bold()
                                    .FontColor(Colors.White);
                                text.Span("  |  ")
                                    .FontSize(9)
                                    .FontColor(Colors.White);
                                text.Span(card.Genre)
                                    .FontSize(9)
                                    .FontColor(Colors.White);
                            });

                        // Center area with album art
                        col.Item()
                            .ExtendVertical()
                            .AlignCenter()
                            .AlignMiddle()
                            .Element(c =>
                            {
                                if (albumImage != null)
                                {
                                    c.Width(35, Unit.Millimetre)
                                        .Height(35, Unit.Millimetre)
                                        .Image(albumImage)
                                        .FitArea();
                                }
                                else
                                {
                                    // Empty placeholder when no album art
                                    c.Width(35, Unit.Millimetre)
                                        .Height(35, Unit.Millimetre);
                                }
                            });

                        // Bottom bar: Artist - Title - Album
                        col.Item()
                            .Height(BarHeight, Unit.Millimetre)
                            .Background("#00000080") // Semi-transparent black
                            .AlignCenter()
                            .AlignMiddle()
                            .Padding(1, Unit.Millimetre)
                            .Text(text =>
                            {
                                text.Span(card.Artist)
                                    .FontSize(7)
                                    .Bold()
                                    .FontColor(Colors.White);
                                text.Span(" - ")
                                    .FontSize(7)
                                    .FontColor(Colors.White);
                                text.Span(card.Title)
                                    .FontSize(7)
                                    .FontColor(Colors.White);
                                if (!string.IsNullOrWhiteSpace(card.AlbumName))
                                {
                                    text.Span(" - ")
                                        .FontSize(7)
                                        .FontColor(Colors.White);
                                    text.Span(card.AlbumName)
                                        .FontSize(7)
                                        .Italic()
                                        .FontColor(Colors.White);
                                }
                            });
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
