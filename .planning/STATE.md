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

Phase: 5 of 9 (Web Foundation)
Plan: Not started
Status: Ready to plan
Last activity: 2025-12-21 - Milestone v2.0 created

Progress: ░░░░░░░░░░ 0% (v2.0)

## Performance Metrics

**Velocity:**
- Total plans completed: 8
- Average duration: 10 min
- Total execution time: 1.42 hours

**By Phase:**

| Phase | Plans | Total | Avg/Plan |
|-------|-------|-------|----------|
| 1 | 1 | 8 min | 8 min |
| 2 | 2 | 32 min | 16 min |
| 3 | 2 | 22 min | 11 min |
| 4 | 3 | 23 min | 8 min |

**Recent Trend:**
- Last 5 plans: 12, 10, 5, 6, 12 min
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
Stopped at: Milestone v2.0 initialization
Resume file: None
