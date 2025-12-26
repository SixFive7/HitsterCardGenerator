using System.Collections.Concurrent;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.Services;

/// <summary>
/// Service for exporting cards to PDF with grid layout for printing
/// </summary>
public static class PdfExporter
{
    private const float CardWidth = 85f;   // mm (landscape)
    private const float CardHeight = 55f;  // mm (landscape)
    private const int Columns = 2;         // 2 cards per row
    private const int Rows = 5;            // 5 rows per page
    private const int CardsPerPage = Columns * Rows; // 10 cards per page

    // A4 page dimensions
    private const float PageWidth = 210f;  // mm
    private const float PageHeight = 297f; // mm

    // Calculated margins to center the grid
    private const float HorizontalMargin = (PageWidth - (Columns * CardWidth)) / 2; // 20mm
    private const float VerticalMargin = (PageHeight - (Rows * CardHeight)) / 2;    // 11mm

    // Cutting line settings
    private const float CuttingLineWidth = 0.5f;  // pt
    private const float CuttingLineExtension = 3f; // mm - extends beyond card edges for alignment

    // Card layout constants
    private const float QrSize = 40f;     // mm - prominent QR code for easy scanning
    private const float BarHeight = 10f;  // mm - height of text bars for better readability

    // Simple in-memory cache for album art during PDF generation
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
    /// Exports cards to a PDF file with front and back pages arranged for double-sided printing
    /// </summary>
    /// <param name="cards">List of card data</param>
    /// <param name="outputPath">Path to save the PDF</param>
    /// <param name="cuttingLineStyle">Style of cutting lines to render</param>
    /// <returns>Total number of pages generated</returns>
    public static int ExportToPdf(List<CardData> cards, string outputPath, CuttingLineStyle cuttingLineStyle = CuttingLineStyle.None)
    {
        var document = Document.Create(container =>
        {
            // Calculate number of sheets needed (each sheet = 1 front page + 1 back page)
            var totalSheets = (int)Math.Ceiling((double)cards.Count / CardsPerPage);

            for (int sheet = 0; sheet < totalSheets; sheet++)
            {
                var startIndex = sheet * CardsPerPage;
                var sheetCards = cards.Skip(startIndex).Take(CardsPerPage).ToList();

                // Front page (QR codes)
                container.Page(page =>
                {
                    page.Size(PageWidth, PageHeight, Unit.Millimetre);
                    page.MarginVertical(VerticalMargin, Unit.Millimetre);
                    page.MarginHorizontal(HorizontalMargin, Unit.Millimetre);

                    page.Content().Layers(layers =>
                    {
                        // Cutting lines layer (behind cards)
                        if (cuttingLineStyle != CuttingLineStyle.None)
                        {
                            var svgContent = GenerateCuttingLinesSvg(cuttingLineStyle, sheetCards.Count);
                            layers.Layer().Svg(svgContent);
                        }

                        // Cards layer
                        layers.PrimaryLayer().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                for (int i = 0; i < Columns; i++)
                                    columns.ConstantColumn(CardWidth, Unit.Millimetre);
                            });

                            for (int row = 0; row < Rows; row++)
                            {
                                for (int colIdx = 0; colIdx < Columns; colIdx++)
                                {
                                    var cardIndex = row * Columns + colIdx;
                                    table.Cell().Element(c =>
                                    {
                                        if (cardIndex < sheetCards.Count)
                                            RenderFrontCard(c, sheetCards[cardIndex]);
                                        else
                                            RenderEmptyCard(c);
                                    });
                                }
                            }
                        });
                    });
                });

                // Back page (year/artist/title - MIRRORED order for duplex)
                container.Page(page =>
                {
                    page.Size(PageWidth, PageHeight, Unit.Millimetre);
                    page.MarginVertical(VerticalMargin, Unit.Millimetre);
                    page.MarginHorizontal(HorizontalMargin, Unit.Millimetre);

                    page.Content().Layers(layers =>
                    {
                        // Cutting lines layer (behind cards)
                        if (cuttingLineStyle != CuttingLineStyle.None)
                        {
                            var svgContent = GenerateCuttingLinesSvg(cuttingLineStyle, sheetCards.Count);
                            layers.Layer().Svg(svgContent);
                        }

                        // Cards layer
                        layers.PrimaryLayer().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                for (int i = 0; i < Columns; i++)
                                    columns.ConstantColumn(CardWidth, Unit.Millimetre);
                            });

                            for (int row = 0; row < Rows; row++)
                            {
                                // Mirror order: reverse the columns for back page
                                for (int colIdx = Columns - 1; colIdx >= 0; colIdx--)
                                {
                                    var cardIndex = row * Columns + colIdx;
                                    table.Cell().Element(c =>
                                    {
                                        if (cardIndex < sheetCards.Count)
                                            RenderBackCard(c, sheetCards[cardIndex]);
                                        else
                                            RenderEmptyCard(c);
                                    });
                                }
                            }
                        });
                    });
                });
            }
        });

        document.GeneratePdf(outputPath);

        // Return total pages (2 pages per sheet)
        var totalSheets2 = (int)Math.Ceiling((double)cards.Count / CardsPerPage);
        return totalSheets2 * 2;
    }

    private static void RenderFrontCard(IContainer container, CardData card)
    {
        var isDark = IsDarkColor(card.BackgroundColor);
        var textColor = isDark ? Colors.White : Colors.Grey.Darken4;

        container
            .Height(CardHeight, Unit.Millimetre)
            .Border(0.25f)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(card.BackgroundColor ?? Colors.White)
            .Column(col =>
            {
                // Top spacer - minimal for larger QR
                col.Item().Height(2, Unit.Millimetre);

                // QR code centered - prominent for easy scanning
                col.Item()
                    .AlignCenter()
                    .Width(QrSize, Unit.Millimetre)
                    .Height(QrSize, Unit.Millimetre)
                    .Image(card.QrCodeData ?? Array.Empty<byte>())
                    .FitArea();

                // Spacer between QR and text
                col.Item().Height(1.5f, Unit.Millimetre);

                // Genre text centered below QR - larger for visibility
                col.Item()
                    .AlignCenter()
                    .Text(card.Genre)
                    .FontSize(11)
                    .Bold()
                    .FontColor(textColor);
            });
    }

    private static void RenderBackCard(IContainer container, CardData card)
    {
        var albumImage = FetchAlbumImage(card.AlbumImageUrl);

        container
            .Height(CardHeight, Unit.Millimetre)
            .Border(0.25f)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(card.BackgroundColor ?? Colors.White)
            .Column(col =>
            {
                // Top bar: Year | Genre - prominent for quick identification
                col.Item()
                    .Height(BarHeight, Unit.Millimetre)
                    .Background("#000000B3") // Semi-transparent black (70% opacity)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text(text =>
                    {
                        text.Span(card.Year.ToString())
                            .FontSize(11)
                            .Bold()
                            .FontColor(Colors.White);
                        text.Span("  |  ")
                            .FontSize(10)
                            .FontColor(Colors.White);
                        text.Span(card.Genre)
                            .FontSize(10)
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
                            c.Width(32, Unit.Millimetre)
                                .Height(32, Unit.Millimetre)
                                .Image(albumImage)
                                .FitArea();
                        }
                        else
                        {
                            // Empty placeholder when no album art
                            c.Width(32, Unit.Millimetre)
                                .Height(32, Unit.Millimetre);
                        }
                    });

                // Bottom bar: Artist - Title - Album
                col.Item()
                    .Height(BarHeight, Unit.Millimetre)
                    .Background("#000000B3") // Semi-transparent black (70% opacity)
                    .AlignCenter()
                    .AlignMiddle()
                    .PaddingHorizontal(2, Unit.Millimetre)
                    .Text(text =>
                    {
                        text.Span(card.Artist)
                            .FontSize(9)
                            .Bold()
                            .FontColor(Colors.White);
                        text.Span(" - ")
                            .FontSize(9)
                            .FontColor(Colors.White);
                        text.Span(card.Title)
                            .FontSize(9)
                            .FontColor(Colors.White);
                        if (!string.IsNullOrWhiteSpace(card.AlbumName))
                        {
                            text.Span(" - ")
                                .FontSize(9)
                                .FontColor(Colors.White);
                            text.Span(card.AlbumName)
                                .FontSize(9)
                                .Italic()
                                .FontColor(Colors.White);
                        }
                    });
            });
    }

    private static void RenderEmptyCard(IContainer container)
    {
        container
            .Height(CardHeight, Unit.Millimetre)
            .Border(0.25f)
            .BorderColor(Colors.Grey.Lighten3);
    }

    /// <summary>
    /// Generates SVG content for cutting lines based on the selected style
    /// </summary>
    private static string GenerateCuttingLinesSvg(CuttingLineStyle style, int cardCount)
    {
        // Grid dimensions in mm (matching the content area)
        var gridWidth = Columns * CardWidth;
        var gridHeight = Rows * CardHeight;

        // Calculate how many rows have cards
        var rowsWithCards = (int)Math.Ceiling((double)cardCount / Columns);

        var svg = new StringBuilder();
        svg.AppendLine($"<svg viewBox=\"0 0 {gridWidth} {gridHeight}\" xmlns=\"http://www.w3.org/2000/svg\">");
        svg.AppendLine($"  <style>line {{ stroke: black; stroke-width: 0.25; }}</style>");

        if (style == CuttingLineStyle.EdgeOnly)
        {
            var bottomY = rowsWithCards * CardHeight;
            // Top edge
            svg.AppendLine($"  <line x1=\"{-CuttingLineExtension}\" y1=\"0\" x2=\"{gridWidth + CuttingLineExtension}\" y2=\"0\" />");
            // Bottom edge
            svg.AppendLine($"  <line x1=\"{-CuttingLineExtension}\" y1=\"{bottomY}\" x2=\"{gridWidth + CuttingLineExtension}\" y2=\"{bottomY}\" />");
            // Left edge
            svg.AppendLine($"  <line x1=\"0\" y1=\"{-CuttingLineExtension}\" x2=\"0\" y2=\"{bottomY + CuttingLineExtension}\" />");
            // Right edge
            svg.AppendLine($"  <line x1=\"{gridWidth}\" y1=\"{-CuttingLineExtension}\" x2=\"{gridWidth}\" y2=\"{bottomY + CuttingLineExtension}\" />");
        }
        else if (style == CuttingLineStyle.Complete)
        {
            // Horizontal lines
            for (int row = 0; row <= rowsWithCards; row++)
            {
                var y = row * CardHeight;
                svg.AppendLine($"  <line x1=\"{-CuttingLineExtension}\" y1=\"{y}\" x2=\"{gridWidth + CuttingLineExtension}\" y2=\"{y}\" />");
            }

            // Vertical lines
            var maxY = rowsWithCards * CardHeight;
            for (int col = 0; col <= Columns; col++)
            {
                var x = col * CardWidth;
                svg.AppendLine($"  <line x1=\"{x}\" y1=\"{-CuttingLineExtension}\" x2=\"{x}\" y2=\"{maxY + CuttingLineExtension}\" />");
            }
        }

        svg.AppendLine("</svg>");
        return svg.ToString();
    }

    /// <summary>
    /// Gets the expected page count for a given number of cards
    /// </summary>
    public static int GetPageCount(int cardCount)
    {
        var sheets = (int)Math.Ceiling((double)cardCount / CardsPerPage);
        return sheets * 2; // Front + back page per sheet
    }
}
