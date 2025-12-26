namespace HitsterCardGenerator.Models;

/// <summary>
/// Request to create a new playlist
/// </summary>
public record CreatePlaylistRequest(string Name);

/// <summary>
/// Request to update a playlist
/// </summary>
public record UpdatePlaylistRequest(string Name);

/// <summary>
/// Summary of a playlist for list views
/// </summary>
public record PlaylistSummary(
    string Id,
    string Name,
    int TrackCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Detailed playlist with tracks
/// </summary>
public record PlaylistDetail(
    string Id,
    string Name,
    List<TrackDto> Tracks,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Track data for API responses
/// </summary>
public record TrackDto(
    string Id,
    string SpotifyId,
    string Title,
    string Artist,
    int Year,
    string Genre,
    string? AlbumArtUrl
);

/// <summary>
/// Request to add a track to a playlist
/// </summary>
public record AddTrackRequest(
    string SpotifyId,
    string Title,
    string Artist,
    int Year,
    string Genre,
    string? AlbumArtUrl
);

/// <summary>
/// Response when a track is added to a playlist
/// </summary>
public record AddTrackResponse(
    string Id,
    string PlaylistId,
    TrackDto Track
);
