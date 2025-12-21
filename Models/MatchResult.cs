namespace HitsterCardGenerator.Models;

public record MatchResult
{
    public int Index { get; init; }
    public string OriginalTitle { get; init; } = string.Empty;
    public string OriginalArtist { get; init; } = string.Empty;
    public int OriginalYear { get; init; }
    public string OriginalGenre { get; init; } = string.Empty;
    public SpotifySearchResult? Match { get; init; }
    public string Confidence { get; init; } = "none"; // "high", "medium", "low", "none"
    public List<SpotifySearchResult> Alternatives { get; init; } = new();
}

public record MatchRequest
{
    public List<SongInput> Songs { get; init; } = new();
}

public record SongInput
{
    public string Title { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public int Year { get; init; }
    public string Genre { get; init; } = string.Empty;
}

public record MatchResponse
{
    public bool Success { get; init; }
    public List<MatchResult> Results { get; init; } = new();
    public int TotalMatched { get; init; }
    public int TotalNotFound { get; init; }
}
