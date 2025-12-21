using HitsterCardGenerator.Services;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.Endpoints;

/// <summary>
/// Spotify track matching endpoints
/// </summary>
public static class MatchEndpoints
{
    public static void MapMatchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api");

        // Match songs to Spotify tracks
        group.MapPost("/match", async (MatchRequest request, IConfiguration config) =>
        {
            try
            {
                // Get Spotify credentials from configuration
                var clientId = config["Spotify:ClientId"];
                var clientSecret = config["Spotify:ClientSecret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    return Results.BadRequest(new MatchResponse
                    {
                        Success = false,
                        Results = new List<MatchResult>()
                    });
                }

                // Create and authenticate SpotifyService
                var spotifyService = new SpotifyService(clientId, clientSecret);
                var authenticated = await spotifyService.AuthenticateAsync();

                if (!authenticated)
                {
                    return Results.BadRequest(new MatchResponse
                    {
                        Success = false,
                        Results = new List<MatchResult>()
                    });
                }

                var results = new List<MatchResult>();
                var index = 0;

                foreach (var song in request.Songs)
                {
                    // Search for the song on Spotify
                    var searchResults = await spotifyService.SearchTrackAsync(song.Artist, song.Title);

                    SpotifySearchResult? match = null;
                    var confidence = "none";
                    var alternatives = new List<SpotifySearchResult>();

                    if (searchResults.Count > 0)
                    {
                        // Select best match using existing smart selection
                        match = spotifyService.SelectBestMatch(searchResults, song.Artist, song.Title);

                        // If SelectBestMatch returns null (ambiguous), pick the first result
                        if (match == null && searchResults.Count > 0)
                        {
                            match = searchResults[0];
                        }

                        // Calculate confidence based on simple string comparison
                        if (match != null)
                        {
                            confidence = CalculateConfidence(
                                song.Title, song.Artist,
                                match.TrackName, match.ArtistName);

                            // Get alternatives (remaining results, excluding the match)
                            alternatives = searchResults
                                .Where(r => r.TrackId != match.TrackId)
                                .Take(4)
                                .ToList();
                        }
                    }

                    results.Add(new MatchResult
                    {
                        Index = index++,
                        OriginalTitle = song.Title,
                        OriginalArtist = song.Artist,
                        OriginalYear = song.Year,
                        OriginalGenre = song.Genre,
                        Match = match,
                        Confidence = confidence,
                        Alternatives = alternatives
                    });
                }

                var matched = results.Count(r => r.Match != null);

                return Results.Ok(new MatchResponse
                {
                    Success = true,
                    Results = results,
                    TotalMatched = matched,
                    TotalNotFound = results.Count - matched
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: 500,
                    title: "Error matching songs to Spotify"
                );
            }
        })
        .WithName("MatchSongs");
    }

    /// <summary>
    /// Calculate match confidence based on string comparison
    /// </summary>
    private static string CalculateConfidence(
        string originalTitle, string originalArtist,
        string matchTitle, string matchArtist)
    {
        var titleMatch = string.Equals(originalTitle, matchTitle, StringComparison.OrdinalIgnoreCase);
        var artistMatch = string.Equals(originalArtist, matchArtist, StringComparison.OrdinalIgnoreCase);

        if (titleMatch && artistMatch)
            return "high";

        if (titleMatch || artistMatch)
            return "medium";

        return "low";
    }
}
