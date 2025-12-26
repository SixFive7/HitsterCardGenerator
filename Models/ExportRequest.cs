using System.Text.Json.Serialization;

namespace HitsterCardGenerator.Models;

/// <summary>
/// Request payload for PDF export endpoint
/// </summary>
public record ExportRequest
{
    /// <summary>
    /// Cards to include in the export (only included cards from frontend)
    /// </summary>
    public required List<ExportCard> Cards { get; init; }

    /// <summary>
    /// Genre to hex color mapping from frontend customization
    /// </summary>
    public required Dictionary<string, string> GenreColors { get; init; }

    /// <summary>
    /// Cutting line style preference
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CuttingLineStyle CuttingLines { get; init; } = CuttingLineStyle.None;
}

/// <summary>
/// Card data for export (matches frontend MatchResult structure)
/// </summary>
public record ExportCard
{
    public required string TrackId { get; init; }
    public required string Title { get; init; }
    public required string Artist { get; init; }
    public required int Year { get; init; }
    public required string Genre { get; init; }
    public string? AlbumImageUrl { get; init; }
    public string? AlbumName { get; init; }
}

/// <summary>
/// Cutting line style options for PDF export
/// </summary>
public enum CuttingLineStyle
{
    /// <summary>No cutting lines (current behavior - light card borders only)</summary>
    None,

    /// <summary>Lines only at page margins (outer edges of grid)</summary>
    EdgeOnly,

    /// <summary>Full grid lines around each card</summary>
    Complete
}
