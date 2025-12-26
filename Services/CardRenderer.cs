using System.Collections.Concurrent;
using SkiaSharp;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.Services;

/// <summary>
/// Service for rendering cards using SkiaSharp for high-quality bitmap output.
/// This is the single source of truth for card rendering - used by both web preview and PDF export.
/// </summary>
public static class CardRenderer
{
    // Card dimensions (credit card size)
    private const float CardWidthMm = 85f;
    private const float CardHeightMm = 55f;

    // Layout constants
    private const float QrSizeMm = 40f;     // QR code size
    private const float BarHeightMm = 10f;  // Height of text bars
    private const float AlbumArtSizeMm = 32f; // Album art size on back

    // Rendering quality
    private const float Dpi = 300f;
    private const float MmToInch = 25.4f;

    // Pixel dimensions at 300 DPI
    private static readonly int CardWidthPx = MmToPixels(CardWidthMm);
    private static readonly int CardHeightPx = MmToPixels(CardHeightMm);
    private static readonly int QrSizePx = MmToPixels(QrSizeMm);
    private static readonly int BarHeightPx = MmToPixels(BarHeightMm);
    private static readonly int AlbumArtSizePx = MmToPixels(AlbumArtSizeMm);

    // Pixel spacings
    private static readonly int TopSpacerPx = MmToPixels(2f);
    private static readonly int QrTextSpacerPx = MmToPixels(1.5f);
    private static readonly int PaddingHorizontalPx = MmToPixels(2f);

    // Font sizes at 300 DPI (points to pixels: pt * DPI / 72)
    private static readonly float FontSize11Px = 11f * Dpi / 72f;
    private static readonly float FontSize10Px = 10f * Dpi / 72f;
    private static readonly float FontSize9Px = 9f * Dpi / 72f;

    // Album image cache
    private static readonly ConcurrentDictionary<string, SKBitmap?> AlbumImageCache = new();
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(10) };

    /// <summary>
    /// Converts millimeters to pixels at 300 DPI
    /// </summary>
    private static int MmToPixels(float mm) => (int)Math.Round(mm * Dpi / MmToInch);

    /// <summary>
    /// Determines if a hex color is dark (for choosing contrasting text color)
    /// </summary>
    public static bool IsDarkColor(string? hexColor)
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
    /// Parses a hex color string to SKColor
    /// </summary>
    private static SKColor ParseHexColor(string? hexColor, SKColor defaultColor)
    {
        if (string.IsNullOrWhiteSpace(hexColor))
            return defaultColor;

        try
        {
            var hex = hexColor.TrimStart('#');

            // Handle RGBA format (8 characters)
            if (hex.Length == 8)
            {
                var r = Convert.ToByte(hex.Substring(0, 2), 16);
                var g = Convert.ToByte(hex.Substring(2, 2), 16);
                var b = Convert.ToByte(hex.Substring(4, 2), 16);
                var a = Convert.ToByte(hex.Substring(6, 2), 16);
                return new SKColor(r, g, b, a);
            }

            // Handle RGB format (6 characters)
            if (hex.Length == 6)
            {
                var r = Convert.ToByte(hex.Substring(0, 2), 16);
                var g = Convert.ToByte(hex.Substring(2, 2), 16);
                var b = Convert.ToByte(hex.Substring(4, 2), 16);
                return new SKColor(r, g, b);
            }

            return defaultColor;
        }
        catch
        {
            return defaultColor;
        }
    }

    /// <summary>
    /// Fetches album image from URL with caching
    /// </summary>
    public static SKBitmap? FetchAlbumImage(string? albumImageUrl)
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
                    var imageData = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                    return SKBitmap.Decode(imageData);
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
    /// Creates an SKFont with the specified parameters
    /// </summary>
    private static SKFont CreateFont(float size, SKFontStyleWeight weight = SKFontStyleWeight.Normal, SKFontStyleSlant slant = SKFontStyleSlant.Upright)
    {
        var typeface = SKTypeface.FromFamilyName("Arial", weight, SKFontStyleWidth.Normal, slant);
        return new SKFont(typeface, size);
    }

    /// <summary>
    /// Renders the front of a card as PNG bytes.
    /// Front shows: QR code centered with genre text below.
    /// </summary>
    public static byte[] RenderFrontCard(CardData card)
    {
        using var bitmap = new SKBitmap(CardWidthPx, CardHeightPx);
        using var canvas = new SKCanvas(bitmap);

        // Background color
        var bgColor = ParseHexColor(card.BackgroundColor, SKColors.White);
        canvas.Clear(bgColor);

        // Text color based on background
        var isDark = IsDarkColor(card.BackgroundColor);
        var textColor = isDark ? SKColors.White : new SKColor(33, 33, 33); // Grey.Darken4 approximation

        // Draw QR code centered
        if (card.QrCodeData != null && card.QrCodeData.Length > 0)
        {
            using var qrBitmap = SKBitmap.Decode(card.QrCodeData);
            if (qrBitmap != null)
            {
                var qrX = (CardWidthPx - QrSizePx) / 2;
                var qrY = TopSpacerPx;
                var destRect = new SKRect(qrX, qrY, qrX + QrSizePx, qrY + QrSizePx);
                canvas.DrawBitmap(qrBitmap, destRect);
            }
        }

        // Draw genre text centered below QR
        if (!string.IsNullOrWhiteSpace(card.Genre))
        {
            using var font = CreateFont(FontSize11Px, SKFontStyleWeight.Bold);
            using var paint = new SKPaint
            {
                Color = textColor,
                IsAntialias = true
            };

            var textX = CardWidthPx / 2f;
            var textY = TopSpacerPx + QrSizePx + QrTextSpacerPx + FontSize11Px;
            canvas.DrawText(card.Genre, textX, textY, SKTextAlign.Center, font, paint);
        }

        // Encode to PNG
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    /// <summary>
    /// Renders the back of a card as PNG bytes.
    /// Back shows: Year/Genre top bar, album art center, Artist/Title/Album bottom bar.
    /// </summary>
    public static byte[] RenderBackCard(CardData card)
    {
        using var bitmap = new SKBitmap(CardWidthPx, CardHeightPx);
        using var canvas = new SKCanvas(bitmap);

        // Background color
        var bgColor = ParseHexColor(card.BackgroundColor, SKColors.White);
        canvas.Clear(bgColor);

        // Semi-transparent black for bars (70% opacity = 0xB3)
        var barColor = new SKColor(0, 0, 0, 0xB3);

        // Top bar: Year | Genre
        using (var barPaint = new SKPaint { Color = barColor })
        {
            canvas.DrawRect(0, 0, CardWidthPx, BarHeightPx, barPaint);
        }

        // Top bar text
        using (var boldFont = CreateFont(FontSize11Px, SKFontStyleWeight.Bold))
        using (var normalFont = CreateFont(FontSize10Px))
        using (var whitePaint = new SKPaint { Color = SKColors.White, IsAntialias = true })
        {
            var yearText = card.Year.ToString();
            var separator = "  |  ";
            var genreText = card.Genre;

            var yearWidth = boldFont.MeasureText(yearText);
            var separatorWidth = normalFont.MeasureText(separator);
            var genreWidth = normalFont.MeasureText(genreText);
            var totalWidth = yearWidth + separatorWidth + genreWidth;

            var startX = (CardWidthPx - totalWidth) / 2f;
            var textY = BarHeightPx / 2f + FontSize11Px / 3f; // Approximate vertical centering

            canvas.DrawText(yearText, startX, textY, SKTextAlign.Left, boldFont, whitePaint);
            canvas.DrawText(separator, startX + yearWidth, textY, SKTextAlign.Left, normalFont, whitePaint);
            canvas.DrawText(genreText, startX + yearWidth + separatorWidth, textY, SKTextAlign.Left, normalFont, whitePaint);
        }

        // Album art in center
        var albumImage = FetchAlbumImage(card.AlbumImageUrl);
        if (albumImage != null)
        {
            var artX = (CardWidthPx - AlbumArtSizePx) / 2;
            var artY = BarHeightPx + (CardHeightPx - 2 * BarHeightPx - AlbumArtSizePx) / 2;
            var destRect = new SKRect(artX, artY, artX + AlbumArtSizePx, artY + AlbumArtSizePx);
            canvas.DrawBitmap(albumImage, destRect);
        }

        // Bottom bar: Artist - Title - Album
        var bottomBarY = CardHeightPx - BarHeightPx;
        using (var barPaint = new SKPaint { Color = barColor })
        {
            canvas.DrawRect(0, bottomBarY, CardWidthPx, BarHeightPx, barPaint);
        }

        // Bottom bar text
        using (var boldFont = CreateFont(FontSize9Px, SKFontStyleWeight.Bold))
        using (var normalFont = CreateFont(FontSize9Px))
        using (var italicFont = CreateFont(FontSize9Px, SKFontStyleWeight.Normal, SKFontStyleSlant.Italic))
        using (var whitePaint = new SKPaint { Color = SKColors.White, IsAntialias = true })
        {
            var artistText = card.Artist ?? "";
            var dash = " - ";
            var titleText = card.Title ?? "";
            var albumText = !string.IsNullOrWhiteSpace(card.AlbumName) ? card.AlbumName : "";

            var artistWidth = boldFont.MeasureText(artistText);
            var dashWidth = normalFont.MeasureText(dash);
            var titleWidth = normalFont.MeasureText(titleText);
            var albumWidth = italicFont.MeasureText(albumText);

            var totalWidth = artistWidth + dashWidth + titleWidth;
            if (!string.IsNullOrWhiteSpace(card.AlbumName))
            {
                totalWidth += dashWidth + albumWidth;
            }

            // Clamp to available width with padding
            var maxWidth = CardWidthPx - 2 * PaddingHorizontalPx;
            var startX = Math.Max(PaddingHorizontalPx, (CardWidthPx - totalWidth) / 2f);
            var textY = bottomBarY + BarHeightPx / 2f + FontSize9Px / 3f;

            var x = startX;
            canvas.DrawText(artistText, x, textY, SKTextAlign.Left, boldFont, whitePaint);
            x += artistWidth;
            canvas.DrawText(dash, x, textY, SKTextAlign.Left, normalFont, whitePaint);
            x += dashWidth;
            canvas.DrawText(titleText, x, textY, SKTextAlign.Left, normalFont, whitePaint);

            if (!string.IsNullOrWhiteSpace(card.AlbumName))
            {
                x += titleWidth;
                canvas.DrawText(dash, x, textY, SKTextAlign.Left, normalFont, whitePaint);
                x += dashWidth;
                canvas.DrawText(albumText, x, textY, SKTextAlign.Left, italicFont, whitePaint);
            }
        }

        // Encode to PNG
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
