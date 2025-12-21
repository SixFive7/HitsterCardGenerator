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
        container
            .Height(CardHeight, Unit.Millimetre)
            .Border(0.25f)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(card.BackgroundColor ?? Colors.White)
            .AlignCenter()
            .AlignMiddle()
            .Padding(5, Unit.Millimetre)
            .Image(card.QrCodeData ?? Array.Empty<byte>())
            .FitArea();
    }

    private static void RenderBackCard(IContainer container, CardData card)
    {
        container
            .Height(CardHeight, Unit.Millimetre)
            .Border(0.25f)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(card.BackgroundColor ?? Colors.White)
            .Padding(3, Unit.Millimetre)
            .AlignCenter()
            .Column(col =>
            {
                // Artist (top, smaller)
                col.Item()
                    .AlignCenter()
                    .Text(card.Artist)
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken2);

                // Spacer
                col.Item().Height(3, Unit.Millimetre);

                // Year (center, large and bold) - the key answer in the game
                col.Item()
                    .AlignCenter()
                    .Text(card.Year.ToString())
                    .FontSize(24)
                    .Bold()
                    .FontColor(Colors.Black);

                // Spacer
                col.Item().Height(3, Unit.Millimetre);

                // Title (below year)
                col.Item()
                    .AlignCenter()
                    .Text(card.Title)
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken3);

                // Spacer
                col.Item().Height(5, Unit.Millimetre);

                // Genre (bottom)
                col.Item()
                    .AlignCenter()
                    .Text(card.Genre)
                    .FontSize(7)
                    .FontColor(Colors.Grey.Medium);
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
