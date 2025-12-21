namespace HitsterCardGenerator.Models;

/// <summary>
/// Represents a track result from Spotify search
/// </summary>
public record SpotifySearchResult
{
    public string TrackId { get; init; } = string.Empty;
    public string TrackName { get; init; } = string.Empty;
    public string ArtistName { get; init; } = string.Empty;
    public string AlbumName { get; init; } = string.Empty;
    public string AlbumType { get; init; } = string.Empty; // "album", "single", "compilation"
    public int ReleaseYear { get; init; }
    public bool IsRemastered { get; init; }
    public string SpotifyUrl => $"https://open.spotify.com/track/{TrackId}";

    /// <summary>
    /// Gets the priority score for smart selection (lower is better)
    /// Priority: album (0) > single (1) > compilation (2)
    /// Add 3 if remastered
    /// </summary>
    public int SelectionPriority
    {
        get
        {
            var basePriority = AlbumType.ToLowerInvariant() switch
            {
                "album" => 0,
                "single" => 1,
                "compilation" => 2,
                _ => 3
            };
            return IsRemastered ? basePriority + 3 : basePriority;
        }
    }
}
