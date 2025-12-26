namespace HitsterCardGenerator.Models;

/// <summary>
/// Data required to design a single card (front and back)
/// </summary>
public record CardData
{
    public string Title { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public int Year { get; init; }
    public string Genre { get; init; } = string.Empty;
    public byte[]? QrCodeData { get; init; }
    public string? BackgroundColor { get; init; }
    public string? AlbumImageUrl { get; init; }
    public string? AlbumName { get; init; }

    /// <summary>
    /// Creates CardData from a Song, optionally with background color
    /// </summary>
    public static CardData FromSong(Song song, bool useBackgroundColor = false)
    {
        return new CardData
        {
            Title = song.Title,
            Artist = song.Artist,
            Year = song.Year,
            Genre = song.Genre,
            QrCodeData = song.QrCodeData,
            BackgroundColor = useBackgroundColor ? GenreColors.GetColor(song.Genre) : null
        };
    }
}
