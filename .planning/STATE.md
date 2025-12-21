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

Phase: 9 of 9 (PDF Export)
Plan: 1 of 2 in current phase
Status: In progress
Last activity: 2025-12-21 - Completed 09-01-PLAN.md

Progress: █████████░ 90% (v2.0)

## Performance Metrics

**Velocity:**
- Total plans completed: 16
- Average duration: 10 min
- Total execution time: 2.91 hours

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
| 9 | 1 | 18 min | 18 min |

**Recent Trend:**
- Last 5 plans: 12, 8, 5, 27, 18 min
- Trend: consistent execution

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

## Session Continuity

Last session: 2025-12-21
Stopped at: Completed 09-01-PLAN.md
Resume file: None
