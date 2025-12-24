using HitsterCardGenerator.Services;
using HitsterCardGenerator.Models;
using SpotifyAPI.Web;

namespace HitsterCardGenerator.Endpoints;

/// <summary>
/// Spotify search endpoints
/// </summary>
public static class SearchEndpoints
{
    public static void MapSearchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api");

        // Search Spotify tracks
        group.MapGet("/search", async (string? q, IConfiguration config) =>
        {
            try
            {
                // Return empty array for empty/whitespace queries
                if (string.IsNullOrWhiteSpace(q))
                {
                    return Results.Ok(new List<SpotifySearchResult>());
                }

                // Get Spotify credentials from configuration
                var clientId = config["Spotify:ClientId"];
                var clientSecret = config["Spotify:ClientSecret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    return Results.BadRequest(new { error = "Spotify credentials not configured" });
                }

                // Create and authenticate SpotifyService
                var spotifyService = new SpotifyService(clientId, clientSecret);
                var authenticated = await spotifyService.AuthenticateAsync();

                if (!authenticated || spotifyService.Client == null)
                {
                    return Results.BadRequest(new { error = "Failed to authenticate with Spotify" });
                }

                // Build search request (free-form query, not filtered)
                var searchRequest = new SearchRequest(SearchRequest.Types.Track, q)
                {
                    Limit = 20
                };

                var response = await spotifyService.Client.Search.Item(searchRequest);

                if (response.Tracks?.Items == null)
                {
                    return Results.Ok(new List<SpotifySearchResult>());
                }

                // Map results to SpotifySearchResult objects
                var results = response.Tracks.Items
                    .Select(track => MapToSearchResult(track))
                    .ToList();

                return Results.Ok(results);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: 500,
                    title: "Error searching Spotify"
                );
            }
        })
        .WithName("SearchSpotify");
    }

    private static SpotifySearchResult MapToSearchResult(FullTrack track)
    {
        var albumType = track.Album?.AlbumType ?? "unknown";
        var artistName = track.Artists?.FirstOrDefault()?.Name ?? "Unknown Artist";
        var albumName = track.Album?.Name ?? "Unknown Album";

        // Parse release year from date (format: YYYY-MM-DD or YYYY)
        var releaseYear = 0;
        if (!string.IsNullOrEmpty(track.Album?.ReleaseDate))
        {
            var datePart = track.Album.ReleaseDate.Split('-')[0];
            int.TryParse(datePart, out releaseYear);
        }

        // Check if remastered (case-insensitive search in track and album name)
        var trackNameLower = track.Name?.ToLowerInvariant() ?? "";
        var albumNameLower = albumName.ToLowerInvariant();
        var isRemastered = trackNameLower.Contains("remaster") || albumNameLower.Contains("remaster");

        return new SpotifySearchResult
        {
            TrackId = track.Id,
            TrackName = track.Name ?? "Unknown Track",
            ArtistName = artistName,
            AlbumName = albumName,
            AlbumType = albumType,
            ReleaseYear = releaseYear,
            IsRemastered = isRemastered,
            AlbumImageUrl = track.Album?.Images?.FirstOrDefault()?.Url ?? string.Empty
        };
    }
}
