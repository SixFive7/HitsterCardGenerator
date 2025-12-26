using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.Services;

/// <summary>
/// Facade for card design operations.
/// Delegates to CardRenderer for actual SkiaSharp rendering.
/// Provides backward-compatible QuestPDF document interface if needed.
/// </summary>
public static class CardDesigner
{
    private const float CardWidth = 85f;  // mm
    private const float CardHeight = 55f; // mm

    /// <summary>
    /// Generates the front card as a PNG byte array using SkiaSharp rendering.
    /// </summary>
    public static byte[] GenerateFrontCardImage(CardData card)
    {
        return CardRenderer.RenderFrontCard(card);
    }

    /// <summary>
    /// Generates the back card as a PNG byte array using SkiaSharp rendering.
    /// </summary>
    public static byte[] GenerateBackCardImage(CardData card)
    {
        return CardRenderer.RenderBackCard(card);
    }

    /// <summary>
    /// Designs the front of a card as a QuestPDF document.
    /// Uses pre-rendered SkiaSharp image embedded in QuestPDF container.
    /// </summary>
    public static IDocument DesignFrontCard(CardData card)
    {
        var imageData = CardRenderer.RenderFrontCard(card);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(CardWidth, CardHeight, Unit.Millimetre);
                page.Margin(0);

                page.Content()
                    .Image(imageData)
                    .FitArea();
            });
        });
    }

    /// <summary>
    /// Designs the back of a card as a QuestPDF document.
    /// Uses pre-rendered SkiaSharp image embedded in QuestPDF container.
    /// </summary>
    public static IDocument DesignBackCard(CardData card)
    {
        var imageData = CardRenderer.RenderBackCard(card);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(CardWidth, CardHeight, Unit.Millimetre);
                page.Margin(0);

                page.Content()
                    .Image(imageData)
                    .FitArea();
            });
        });
    }
}
