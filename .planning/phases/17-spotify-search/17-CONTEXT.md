# Phase 17: Spotify Search - Context

**Gathered:** 2025-12-24
**Status:** Ready for planning

<vision>
## How This Should Work

A simple, clean search box where users type a query and get a list of matching Spotify tracks. No complicated filters or advanced options - just type and see results.

Each result shows the track title, artist, year, and album artwork thumbnail. It should feel fast and responsive - results appearing quickly as you search.

The search experience should blend seamlessly with the existing app aesthetic (Spotify-inspired colors, card-based UI from the preview flow).

</vision>

<essential>
## What Must Be Nailed

- **Speed** - Results appear fast, no waiting or loading delays
- **Accuracy** - Easy to find the exact right version of a song
- **Simplicity** - Dead simple to use, zero learning curve

</essential>

<boundaries>
## What's Out of Scope

- Playlist management (adding/removing tracks) - that's Phase 18
- Audio previews / play buttons - keep it visual only
- Advanced filters (genre, year, etc.) - just a simple search query
- Any track selection/action - Phase 17 is purely display

</boundaries>

<specifics>
## Specific Ideas

- Search results should match the existing app aesthetic (Spotify-inspired colors, dark theme)
- Album art thumbnails with each result for visual recognition
- Simple search box UI - nothing fancy

</specifics>

<notes>
## Additional Context

This phase builds on the SpotifyAPI.Web integration from Phase 3. The backend already has Spotify authentication working, so this adds the search endpoint and a new UI component.

Phase 18 will add the ability to select and manage tracks from search results.

</notes>

---

*Phase: 17-spotify-search*
*Context gathered: 2025-12-24*
