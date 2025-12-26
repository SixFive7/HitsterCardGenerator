# Phase 34: Playlist Selection UI - Context

## Vision

Add a playlist selection interface to the landing page. Users see their existing playlists and can create new ones or select an existing playlist to work with. On first visit, a default "My Playlist" is auto-created for a seamless experience.

## Essential

### Browser UUID Generation
- Generate UUID on first visit using `crypto.randomUUID()`
- Store in localStorage as `hitster-browser-id`
- Send as `X-Browser-Id` header on all API calls

### Landing Page Playlist Section
- Show list of user's playlists below the existing hero/input area
- Each playlist card shows: name, track count, last updated
- Click to select and proceed to that playlist's flow
- "Create New Playlist" button/card

### First Visit Experience
- Check if playlists exist for this browser
- If none: auto-create "My Playlist" and proceed with it selected
- User sees their new playlist immediately, no extra clicks

### Playlist State Management
- Store currently selected playlist ID in Svelte store
- Store browser ID in Svelte store for API calls
- Persist selected playlist ID to localStorage

## UI Components

```
PlaylistCard.svelte - Individual playlist display
PlaylistList.svelte - Grid/list of playlist cards
CreatePlaylistModal.svelte - Simple modal for new playlist name
```

## Boundaries (Out of Scope)

- Integration with CSV/Spotify flows (Phase 35)
- Rename/delete playlist UI (Phase 36)
- Error toasts, loading skeletons (Phase 36)

## Notes

- Use existing Tailwind styling patterns from the app
- Playlists should feel like a natural extension of landing page
- Keep UI minimal - just enough to select/create playlists
