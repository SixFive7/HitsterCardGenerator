# Phase 19: Flow Integration - Context

**Gathered:** 2025-12-24
**Status:** Ready for planning

<vision>
## How This Should Work

Users arrive at a landing page with a hero section (app title/welcome) and two clear path options below: "Upload CSV" or "Build Playlist". Both options have equal visual weight - neither is emphasized as the "recommended" way.

When a user picks a path, they navigate away from the landing completely - no toggle or back button to switch modes mid-flow.

The "Build Playlist" path flows straight to card preview after building - no matching step since tracks are already from Spotify. The "CSV Upload" path stays mostly the same (CSV → Match → Preview), with minor tweaks if needed to feel consistent with the new landing page.

For playlist-built tracks, genres come from Spotify metadata. If a track doesn't have clear genre data, the user is prompted to pick a genre when adding that track.

The whole experience matches the current Spotify-inspired dark theme with green accents.

</vision>

<essential>
## What Must Be Nailed

- **Clean landing experience** - Users immediately understand their two options on arrival
- **Seamless flow to preview** - Both paths lead smoothly to the same card preview step
- **Skipping Spotify matching** - Playlist-built tracks don't need re-matching since they're already from Spotify

</essential>

<boundaries>
## What's Out of Scope

- Saving/loading playlists - just build and generate in one session
- Mixing modes - can't combine CSV tracks with manually added tracks
- Mode switching mid-flow - once you pick a path, you commit to it

</boundaries>

<specifics>
## Specific Ideas

- Hero section at top with app title, two path options below
- Match current Spotify-inspired dark theme (already established)
- Equal visual weight to both options
- Genre picker prompt appears when adding tracks without Spotify genre data

</specifics>

<notes>
## Additional Context

The CSV flow may need minor tweaks to feel unified with the new landing page, but the core flow (Upload → Match → Preview) stays the same.

This phase completes the v2.4 Features milestone by connecting the new playlist builder to the existing card generation workflow.

</notes>

---

*Phase: 19-flow-integration*
*Context gathered: 2025-12-24*
