using SpotifyAPI.Web;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.Services;

public class SpotifyService
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private SpotifyClient? _client;

    public SpotifyService(string clientId, string clientSecret)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    /// <summary>
    /// Gets whether the service has successfully authenticated
    /// </summary>
    public bool IsAuthenticated => _client != null;

    /// <summary>
    /// Gets the authenticated Spotify client for API calls
    /// </summary>
    public SpotifyClient? Client => _client;

    /// <summary>
    /// Authenticates with Spotify using client credentials flow
    /// </summary>
    /// <returns>True if authentication succeeded, false otherwise</returns>
    public async Task<bool> AuthenticateAsync()
    {
        try
        {
            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(_clientId, _clientSecret);
            var response = await new OAuthClient(config).RequestToken(request);

            _client = new SpotifyClient(config.WithToken(response.AccessToken));
            return true;
        }
        catch (APIException)
        {
            // Authentication failed (invalid credentials)
            _client = null;
            return false;
        }
        catch (Exception)
        {
            // Network or other error
            _client = null;
            return false;
        }
    }

    /// <summary>
    /// Searches Spotify for tracks matching the given artist and title
    /// </summary>
    /// <param name="artist">Artist name to search for</param>
    /// <param name="title">Track title to search for</param>
    /// <returns>List of search results, empty if no results or not authenticated</returns>
    public async Task<List<SpotifySearchResult>> SearchTrackAsync(string artist, string title)
    {
        if (_client == null)
            return new List<SpotifySearchResult>();

        try
        {
            // Build search query with artist and track filters
            var query = $"track:{title} artist:{artist}";
            var searchRequest = new SearchRequest(SearchRequest.Types.Track, query)
            {
                Limit = 10
            };

            var response = await _client.Search.Item(searchRequest);

            if (response.Tracks?.Items == null)
                return new List<SpotifySearchResult>();

            return response.Tracks.Items
                .Select(track => MapToSearchResult(track))
                .ToList();
        }
        catch (Exception)
        {
            return new List<SpotifySearchResult>();
        }
    }

    /// <summary>
    /// Selects the best match from search results using smart selection logic
    /// </summary>
    /// <param name="results">Search results to select from</param>
    /// <param name="artist">Original artist for matching</param>
    /// <param name="title">Original title for matching</param>
    /// <returns>Best match, or null if ambiguous (multiple equal-priority matches)</returns>
    public SpotifySearchResult? SelectBestMatch(List<SpotifySearchResult> results, string artist, string title)
    {
        if (results.Count == 0)
            return null;

        if (results.Count == 1)
            return results[0];

        // Sort by selection priority (lower is better)
        var sorted = results
            .OrderBy(r => r.SelectionPriority)
            .ThenBy(r => r.ReleaseYear) // Prefer earlier releases
            .ToList();

        var best = sorted[0];

        // Check if ambiguous: multiple results with same priority
        if (sorted.Count > 1 && sorted[1].SelectionPriority == best.SelectionPriority)
        {
            // Ambiguous - multiple equal-priority matches
            return null;
        }

        return best;
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
