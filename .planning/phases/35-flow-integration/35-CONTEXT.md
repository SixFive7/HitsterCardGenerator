# Phase 35: Flow Integration - Context

## Vision

Connect the existing CSV import and Spotify search flows to playlist persistence. Tracks automatically save to the currently selected playlist, creating a unified experience where all tracks end up persisted regardless of input method.

## Essential

### CSV Import Flow Integration
- After Spotify matching completes, save matched tracks to selected playlist
- Update track store to sync with playlist API
- Existing card preview shows tracks from playlist

### Spotify Search Flow Integration
- When adding track via search, save to selected playlist immediately
- Remove temporary in-memory-only playlist concept
- Search results add to persistent playlist

### Track Store Updates
- Sync cardStore with playlist tracks from API
- Load tracks from selected playlist on app start
- Save changes back to API

### Card Preview Integration
- Preview shows tracks from selected playlist
- PDF export uses tracks from selected playlist
- Maintain existing card rendering logic

## Boundaries (Out of Scope)

- Playlist rename/delete UI (Phase 36)
- Error toasts for failed saves (Phase 36)
- Loading states during sync (Phase 36)
- Offline support

## Notes

- Keep existing card preview/export logic intact
- Focus on connecting data flow, not changing UI
- Playlist selection already done in Phase 34
- Track deduplication handled by API (GetOrCreate pattern)
