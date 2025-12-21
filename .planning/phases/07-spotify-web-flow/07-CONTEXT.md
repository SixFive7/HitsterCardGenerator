# Phase 7: Spotify Web Flow - Context

**Gathered:** 2025-12-21
**Status:** Ready for planning

<vision>
## How This Should Work

After a successful CSV upload and validation, matching starts automatically — no extra clicks needed. The system uses host-provided Spotify credentials (not user OAuth) to search for all uploaded songs in parallel.

Users see a progress indicator while matching runs, then results appear showing each song with its matched Spotify track. Album art makes it visual and easy to scan. Confidence indicators show how certain each match is — exact match, close match, or uncertain.

When a match is wrong, clicking it reveals 3-5 alternatives. One click on an alternative swaps it in. Simple and fast.

Songs that can't be found show clearly as "Not found" — the user can skip them or browse alternatives, but they're not blocked from proceeding.

</vision>

<essential>
## What Must Be Nailed

- **Match accuracy** — Finding the RIGHT Spotify track (correct version, not remasters/covers)
- **Speed & feedback** — Fast matching with clear progress indication during the auto-match phase
- **Easy corrections** — Simple click-to-select when the wrong track was matched

</essential>

<boundaries>
## What's Out of Scope

- Manual search — no free-text search box to find tracks manually (only auto-match + alternatives)
- Track previews — no audio playback to verify tracks (just show track info and album art)
- Batch editing — no multi-select to change several tracks at once

</boundaries>

<specifics>
## Specific Ideas

- Album art displayed alongside each matched track
- Confidence indicators: exact match, close match, uncertain
- Show 3-5 alternative suggestions when user wants to change a match
- Click-to-select for corrections (no modals/dropdowns, just click alternative)
- Automatic transition from Phase 6 upload → matching (with progress indicator)
- "Not found" state for unmatched songs (visible but not blocking)

**Credentials:**
- Host-provided credentials via .env file (gitignored)
- Will use environment variables in production containers
- No user-facing OAuth — reuses existing Client Credentials flow from v1.0

</specifics>

<notes>
## Additional Context

The roadmap suggested OAuth redirect flow, but the actual need is simpler: host provides credentials, users just see the matching UI. This reuses the SpotifyService from v1.0 which already implements Client Credentials flow and smart track selection (preferring albums over singles, avoiding remasters).

User provided Spotify API credentials to store in project:
- Client ID: ***REMOVED***
- Client Secret: ***REMOVED***

</notes>

---

*Phase: 07-spotify-web-flow*
*Context gathered: 2025-12-21*
