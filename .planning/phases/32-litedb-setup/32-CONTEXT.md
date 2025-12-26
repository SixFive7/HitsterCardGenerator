# Phase 32: LiteDB Setup - Context

## Vision

Set up LiteDB as the persistence layer for the Hitster Card Generator with a clean repository pattern. The database will store playlists and tracks as separate collections with references, enabling multi-playlist management per browser session.

## Essential

- **Database Location:** `./data/hitster.db` (relative to app root, Docker-friendly)
- **Initialization:** Singleton `LiteDatabase` instance for application lifetime
- **Data Model:** Separate collections for Playlists and Tracks with references (normalized approach)
- **Repository Pattern:** `IPlaylistRepository` with CRUD operations needed for milestone
- **Browser Identity:** Playlists associated with browser UUID (generated in Phase 34)

## Data Model

```
Playlist:
  - Id: ObjectId (primary key)
  - BrowserId: string (UUID for browser identity)
  - Name: string
  - CreatedAt: DateTime
  - UpdatedAt: DateTime
  - TrackIds: List<ObjectId> (references to Track collection)

Track:
  - Id: ObjectId (primary key)
  - SpotifyId: string
  - Title: string
  - Artist: string
  - Year: int
  - Genre: string
  - AlbumArtUrl: string
  - CreatedAt: DateTime
```

## Boundaries (Out of Scope)

- API endpoints (Phase 33)
- Browser UUID generation/management (Phase 34)
- UI components (Phase 34)
- Integration with existing card flow (Phase 35)
- Error handling/retry logic, loading states (Phase 36)

## Notes

- LiteDB is a serverless embedded NoSQL database - no external dependencies
- Normalized data model chosen over embedded for flexibility in track reuse
- Repository pattern allows easy mocking for tests
- Singleton pattern is LiteDB's recommended approach for web apps
