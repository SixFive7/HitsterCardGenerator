namespace HitsterCardGenerator.Models;

/// <summary>
/// Response metadata for PDF export (returned in header, not body since body is PDF file)
/// </summary>
public record ExportResponse
{
    /// <summary>
    /// Whether the export was successful
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Generated filename for the PDF
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Number of cards in the export
    /// </summary>
    public required int CardCount { get; init; }

    /// <summary>
    /// Number of pages in the PDF
    /// </summary>
    public required int PageCount { get; init; }
}
