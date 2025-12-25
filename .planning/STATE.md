# Project State

## Project Summary

**Building:** A .NET 10 console application that generates printable PDF cards for a custom Hitster-style music guessing game, using Spectre.Console for a wizard-like experience.

**Core requirements:**
- Import semicolon-separated CSV with title, artist, year, genre
- Validate CSV and genres against 30+ popular genres + 5 French genres
- Authenticate with Spotify and search for track IDs
- Generate QR codes linking to Spotify tracks
- Export PDF with credit-card sized cards (85x55mm), 16 per A4 page

**Constraints:**
- .NET 10 (Minimal API backend + existing services)
- Svelte + Tailwind frontend (Vite)
- Cross-platform (Windows, Mac, Linux)
- Libraries: QuestPDF, QRCoder, SpotifyAPI.Web (reused from v1.0)

## Current Position

Phase: 20 of 21 (Remove CI/CD)
Plan: 1 of 1 in current phase
Status: Phase complete
Last activity: 2025-12-25 - Completed 20-01-PLAN.md

Progress: ██████████ 100%

## Performance Metrics

**Velocity:**
- Total plans completed: 29
- Average duration: 9 min
- Total execution time: 4h 23m

**By Phase:**

| Phase | Plans | Total | Avg/Plan |
|-------|-------|-------|----------|
| 1 | 1 | 8 min | 8 min |
| 2 | 2 | 32 min | 16 min |
| 3 | 2 | 22 min | 11 min |
| 4 | 3 | 23 min | 8 min |
| 5 | 2 | 12 min | 6 min |
| 6 | 1 | 8 min | 8 min |
| 7 | 2 | 20 min | 10 min |
| 8 | 2 | 32 min | 16 min |
| 9 | 2 | 64 min | 32 min |
| 10 | 1 | 3 min | 3 min |
| 11 | 1 | 4 min | 4 min |
| 12 | 1 | 5 min | 5 min |
| 13 | 1 | 4 min | 4 min |
| 14 | 1 | 3 min | 3 min |
| 15 | 1 | 1 min | 1 min |
| 16 | 2 | 5 min | 2.5 min |
| 17 | 1 | 5 min | 5 min |
| 18 | 1 | 5 min | 5 min |
| 19 | 1 | 6 min | 6 min |
| 20 | 1 | 1 min | 1 min |

**Recent Trend:**
- Last 5 plans: 4, 5, 5, 6, 1 min
- Trend: Consistent fast execution with subagent delegation

*Updated after each plan completion*

## Accumulated Context

### Decisions Made

| Phase | Decision | Rationale |
|-------|----------|-----------|
| 0 | Semicolon CSV separator | Song titles may contain commas |
| 0 | 85x55mm card size | Standard Hitster card size, 16 fit on A4 |
| 0 | QuestPDF for PDF generation | Modern, fluent API, good for complex layouts |
| 0 | QRCoder for QR codes | Simple, well-maintained, cross-platform |
| 1 | .NET 10 target framework | Latest LTS for long-term support |
| 1 | Standard FIGlet font | Best readability across terminals |
| 1 | 30-char step panel width | Proper alignment for step names |
| 2 | Columns instead of Layout | Layout fills screen, breaks prompt display |
| 2 | Prompts inside content panels | Better UX - prompts flow naturally with step content |
| 3 | SpotifyAPI.Web library | Standard .NET Spotify client, well-maintained |
| 3 | Client credentials flow | No user auth needed for track search |
| 3 | Smart selection priority | album > single > compilation, non-remastered preferred |
| 4 | Card grid layout | 2x5 (10 per A4) - original 4x4 (16) doesn't fit with 85x55mm cards |
| 5 | @tailwindcss/vite plugin | Zero-config Tailwind v4 setup with Vite |
| 5 | Spotify-inspired colors | #1DB954 green, #FF6B6B coral, #191414 dark surface |
| 5 | Svelte 5 runes | Modern reactivity pattern ($state, $effect) |
| 8 | Color Palettes First | Preset palettes before per-genre customization |
| 8 | External Control Pattern | CardCarousel accepts external currentIndex and flippedCards props |
| 8 | localStorage Keys | 'hitster-genre-colors' and 'hitster-included-cards' |
| 9 | SVG-based Cutting Lines | QuestPDF Canvas API deprecated in 2024.3.0, SVG approach is future-proof |
| 10 | Standard middleware order | UseDefaultFiles → UseStaticFiles → routes → MapFallbackToFile |
| 11 | Vite direct output to wwwroot | Eliminates copy step, cleaner workflow |
| 11 | Simplified NpmInstall condition | MSBuild datetime comparison unreliable, use Exists() check |
| 12 | Simple devcontainer (no firewall) | Faster setup, relies on container isolation |
| 12 | .NET 10 devcontainer base image | Official MS image with SDK pre-installed |
| 12 | Claude config volume mount | Persists auth across container rebuilds |
| 13 | Release-only NpmBuild | Debug builds fast, Release includes frontend |
| 13 | No serverReadyAction | Developer opens Vite port 5173 for HMR |
| 13 | Compound launch first | Default F5 starts full stack environment |
| 14 | wget over curl in HEALTHCHECK | aspnet image is minimal, wget more likely available |
| 14 | Port 8080 (non-root) | Ports below 1024 require root privileges |
| 14 | Runtime env vars for secrets | SPOTIFY_CLIENT_ID/SECRET passed at docker run, not baked in |

### Deferred Issues

None yet.

### Blockers/Concerns Carried Forward

None yet.

## Project Alignment

Last checked: Project start
Status: ✓ Aligned
Assessment: No work done yet - baseline alignment.
Drift notes: None

### Roadmap Evolution

- Milestone v2.0 created: Web Interface transformation, 5 phases (Phase 5-9)
- Milestone v2.1 created: Enhancements - single-process startup, 2 phases (Phase 10-11)
- Milestone v2.2 created: Polish - devcontainer for Claude Code, 1 phase (Phase 12)
- Milestone v2.2 archived: 2025-12-23
- Milestone v2.3 created: Containerization (Dev Experience + Docker + CI/CD + multi-arch), 4 phases (Phase 13-16)
- Milestone v2.4 created: Features - Playlist Builder as CSV alternative, 3 phases (Phase 17-19)
- Milestone v2.5 created: Self-Hosting - Remove CI/CD and GHCR dependency, 2 phases (Phase 20-21)

## Session Continuity

Last session: 2025-12-25
Stopped at: Completed 20-01-PLAN.md (Phase 20 complete)
Resume file: None
