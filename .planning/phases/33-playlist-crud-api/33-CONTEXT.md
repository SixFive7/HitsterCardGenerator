# Phase 33: Playlist CRUD API - Context

## Vision

Create RESTful API endpoints for complete playlist management. The API enables browsers to create, read, update, and delete playlists, plus manage tracks within playlists. Browser identity is handled via an explicit header for simplicity.

## Essential

### API Endpoints

```
Playlists:
  GET    /api/playlists              - List all playlists for browser
  GET    /api/playlists/{id}         - Get single playlist with tracks
  POST   /api/playlists              - Create new playlist
  PUT    /api/playlists/{id}         - Update playlist (name, etc.)
  DELETE /api/playlists/{id}         - Delete playlist

Tracks within Playlist:
  POST   /api/playlists/{id}/tracks  - Add track(s) to playlist
  DELETE /api/playlists/{id}/tracks/{trackId} - Remove track from playlist
```

### Browser Identity

- **Header-based:** `X-Browser-Id` header on all requests
- Client generates UUID on first visit (stored in localStorage)
- Explicit and simple to reason about
- No cookies, no magic

### Request/Response Format

```json
// POST /api/playlists
Request: { "name": "My Playlist" }
Response: { "id": "...", "name": "My Playlist", "trackCount": 0, "createdAt": "..." }

// GET /api/playlists/{id}
Response: {
  "id": "...",
  "name": "...",
  "tracks": [...],
  "createdAt": "...",
  "updatedAt": "..."
}

// POST /api/playlists/{id}/tracks
Request: { "spotifyId": "...", "title": "...", "artist": "...", ... }
Response: { "id": "...", "playlistId": "...", "track": {...} }
```

## Boundaries (Out of Scope)

- Browser UUID generation in frontend (Phase 34)
- Playlist selection UI (Phase 34)
- Integration with existing card flow (Phase 35)
- Error handling UX, loading states (Phase 36)

## Notes

- Use existing repository pattern from Phase 32
- Return appropriate HTTP status codes (201 Created, 404 Not Found, etc.)
- Validate browser ID is present on all playlist operations
- Use minimal API endpoint mapping (not controllers)
