using HitsterCardGenerator.Models;
using HitsterCardGenerator.Repositories;
using LiteDB;

namespace HitsterCardGenerator.Endpoints;

/// <summary>
/// Playlist management endpoints
/// </summary>
public static class PlaylistEndpoints
{
    private const string BrowserIdHeader = "X-Browser-Id";

    public static void MapPlaylistEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/playlists");

        // GET /api/playlists - List all playlists for browser
        group.MapGet("/", (HttpContext context, IPlaylistRepository playlistRepo) =>
        {
            var browserId = GetBrowserId(context);
            if (browserId == null)
            {
                return Results.BadRequest(new { error = "X-Browser-Id header is required" });
            }

            var playlists = playlistRepo.GetByBrowserId(browserId)
                .Select(p => new PlaylistSummary(
                    p.Id.ToString(),
                    p.Name,
                    p.TrackIds.Count,
                    p.CreatedAt,
                    p.UpdatedAt
                ))
                .ToList();

            return Results.Ok(playlists);
        })
        .WithName("GetPlaylists");

        // GET /api/playlists/{id} - Get single playlist with tracks
        group.MapGet("/{id}", (string id, HttpContext context,
            IPlaylistRepository playlistRepo, ITrackRepository trackRepo) =>
        {
            var browserId = GetBrowserId(context);
            if (browserId == null)
            {
                return Results.BadRequest(new { error = "X-Browser-Id header is required" });
            }

            if (!TryParseObjectId(id, out var objectId))
            {
                return Results.NotFound(new { error = "Playlist not found" });
            }

            var playlist = playlistRepo.GetById(objectId);
            if (playlist == null || playlist.BrowserId != browserId)
            {
                return Results.NotFound(new { error = "Playlist not found" });
            }

            var tracks = trackRepo.GetByIds(playlist.TrackIds)
                .Select(t => new TrackDto(
                    t.Id.ToString(),
                    t.SpotifyId,
                    t.Title,
                    t.Artist,
                    t.Year,
                    t.Genre,
                    t.AlbumArtUrl
                ))
                .ToList();

            var detail = new PlaylistDetail(
                playlist.Id.ToString(),
                playlist.Name,
                tracks,
                playlist.CreatedAt,
                playlist.UpdatedAt
            );

            return Results.Ok(detail);
        })
        .WithName("GetPlaylist");

        // POST /api/playlists - Create new playlist
        group.MapPost("/", (CreatePlaylistRequest request, HttpContext context,
            IPlaylistRepository playlistRepo) =>
        {
            var browserId = GetBrowserId(context);
            if (browserId == null)
            {
                return Results.BadRequest(new { error = "X-Browser-Id header is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest(new { error = "Name is required" });
            }

            var playlist = new Playlist
            {
                BrowserId = browserId,
                Name = request.Name.Trim()
            };

            var id = playlistRepo.Create(playlist);

            var summary = new PlaylistSummary(
                id.ToString(),
                playlist.Name,
                0,
                playlist.CreatedAt,
                playlist.UpdatedAt
            );

            return Results.Created($"/api/playlists/{id}", summary);
        })
        .WithName("CreatePlaylist");

        // PUT /api/playlists/{id} - Update playlist
        group.MapPut("/{id}", (string id, UpdatePlaylistRequest request, HttpContext context,
            IPlaylistRepository playlistRepo) =>
        {
            var browserId = GetBrowserId(context);
            if (browserId == null)
            {
                return Results.BadRequest(new { error = "X-Browser-Id header is required" });
            }

            if (!TryParseObjectId(id, out var objectId))
            {
                return Results.NotFound(new { error = "Playlist not found" });
            }

            var playlist = playlistRepo.GetById(objectId);
            if (playlist == null || playlist.BrowserId != browserId)
            {
                return Results.NotFound(new { error = "Playlist not found" });
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest(new { error = "Name is required" });
            }

            playlist.Name = request.Name.Trim();
            playlistRepo.Update(playlist);

            var summary = new PlaylistSummary(
                playlist.Id.ToString(),
                playlist.Name,
                playlist.TrackIds.Count,
                playlist.CreatedAt,
                playlist.UpdatedAt
            );

            return Results.Ok(summary);
        })
        .WithName("UpdatePlaylist");

        // DELETE /api/playlists/{id} - Delete playlist
        group.MapDelete("/{id}", (string id, HttpContext context, IPlaylistRepository playlistRepo) =>
        {
            var browserId = GetBrowserId(context);
            if (browserId == null)
            {
                return Results.BadRequest(new { error = "X-Browser-Id header is required" });
            }

            if (!TryParseObjectId(id, out var objectId))
            {
                return Results.NotFound(new { error = "Playlist not found" });
            }

            var playlist = playlistRepo.GetById(objectId);
            if (playlist == null || playlist.BrowserId != browserId)
            {
                return Results.NotFound(new { error = "Playlist not found" });
            }

            playlistRepo.Delete(objectId);
            return Results.NoContent();
        })
        .WithName("DeletePlaylist");

        // POST /api/playlists/{id}/tracks - Add track to playlist
        group.MapPost("/{id}/tracks", (string id, AddTrackRequest request, HttpContext context,
            IPlaylistRepository playlistRepo, ITrackRepository trackRepo) =>
        {
            var browserId = GetBrowserId(context);
            if (browserId == null)
            {
                return Results.BadRequest(new { error = "X-Browser-Id header is required" });
            }

            if (!TryParseObjectId(id, out var playlistId))
            {
                return Results.NotFound(new { error = "Playlist not found" });
            }

            var playlist = playlistRepo.GetById(playlistId);
            if (playlist == null || playlist.BrowserId != browserId)
            {
                return Results.NotFound(new { error = "Playlist not found" });
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.SpotifyId))
            {
                return Results.BadRequest(new { error = "SpotifyId is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest(new { error = "Title is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Artist))
            {
                return Results.BadRequest(new { error = "Artist is required" });
            }

            // Create or reuse existing track
            var track = new Track
            {
                SpotifyId = request.SpotifyId,
                Title = request.Title.Trim(),
                Artist = request.Artist.Trim(),
                Year = request.Year,
                Genre = request.Genre?.Trim() ?? string.Empty,
                AlbumArtUrl = request.AlbumArtUrl
            };

            var trackId = trackRepo.GetOrCreate(track);
            var savedTrack = trackRepo.GetById(trackId);

            // Add track to playlist if not already present
            if (!playlist.TrackIds.Contains(trackId))
            {
                playlist.TrackIds.Add(trackId);
                playlistRepo.Update(playlist);
            }

            var trackDto = new TrackDto(
                savedTrack!.Id.ToString(),
                savedTrack.SpotifyId,
                savedTrack.Title,
                savedTrack.Artist,
                savedTrack.Year,
                savedTrack.Genre,
                savedTrack.AlbumArtUrl
            );

            var response = new AddTrackResponse(
                trackId.ToString(),
                playlist.Id.ToString(),
                trackDto
            );

            return Results.Created($"/api/playlists/{id}/tracks/{trackId}", response);
        })
        .WithName("AddTrackToPlaylist");

        // DELETE /api/playlists/{id}/tracks/{trackId} - Remove track from playlist
        group.MapDelete("/{id}/tracks/{trackId}", (string id, string trackId, HttpContext context,
            IPlaylistRepository playlistRepo) =>
        {
            var browserId = GetBrowserId(context);
            if (browserId == null)
            {
                return Results.BadRequest(new { error = "X-Browser-Id header is required" });
            }

            if (!TryParseObjectId(id, out var playlistObjectId))
            {
                return Results.NotFound(new { error = "Playlist not found" });
            }

            if (!TryParseObjectId(trackId, out var trackObjectId))
            {
                return Results.NotFound(new { error = "Track not found" });
            }

            var playlist = playlistRepo.GetById(playlistObjectId);
            if (playlist == null || playlist.BrowserId != browserId)
            {
                return Results.NotFound(new { error = "Playlist not found" });
            }

            // Remove track from playlist
            if (playlist.TrackIds.Contains(trackObjectId))
            {
                playlist.TrackIds.Remove(trackObjectId);
                playlistRepo.Update(playlist);
            }

            return Results.NoContent();
        })
        .WithName("RemoveTrackFromPlaylist");
    }

    private static string? GetBrowserId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(BrowserIdHeader, out var values))
        {
            var browserId = values.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(browserId))
            {
                return browserId;
            }
        }
        return null;
    }

    private static bool TryParseObjectId(string value, out ObjectId objectId)
    {
        try
        {
            objectId = new ObjectId(value);
            return true;
        }
        catch
        {
            objectId = ObjectId.Empty;
            return false;
        }
    }
}
