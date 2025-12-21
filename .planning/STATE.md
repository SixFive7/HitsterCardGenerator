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
- .NET 10 console application
- Spectre.Console for all user interaction
- Cross-platform (Windows, Mac, Linux)
- Libraries: Spectre.Console, QuestPDF, QRCoder

## Current Position

Phase: 4 of 4 (Card Generation)
Plan: 2 of 3 in current phase
Status: In progress
Last activity: 2025-12-21 - Completed 04-02-PLAN.md

Progress: █████████░ 90%

## Performance Metrics

**Velocity:**
- Total plans completed: 7
- Average duration: 10 min
- Total execution time: 1.22 hours

**By Phase:**

| Phase | Plans | Total | Avg/Plan |
|-------|-------|-------|----------|
| 1 | 1 | 8 min | 8 min |
| 2 | 2 | 32 min | 16 min |
| 3 | 2 | 22 min | 11 min |
| 4 | 2 | 11 min | 6 min |

**Recent Trend:**
- Last 5 plans: 27, 12, 10, 5, 6 min
- Trend: fast execution

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

## Session Continuity

Last session: 2025-12-21
Stopped at: Completed 04-02-PLAN.md
Resume file: .planning/phases/04-card-generation/04-03-PLAN.md
