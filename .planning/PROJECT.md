# Hitster Card Generator

## Current State (Updated: 2025-12-22)

**Shipped:** v2.1 Enhancements (2025-12-22)
**Status:** Production-ready
**Codebase:** ~10,500 lines (C# backend + Svelte/TypeScript frontend)
**Stack:** .NET 10 Minimal API, Svelte 5, Tailwind v4, QuestPDF, QRCoder

**v2.1 Improvements:**
- Single `dotnet run` builds and serves complete application
- MSBuild automates npm install/build on first run
- No separate frontend dev server needed

The application is complete with both console and web interfaces:

**Console (v1.0):**
- Import semicolon-separated CSV files with song data
- Validate genres against 35 supported genres
- Authenticate with Spotify API (client credentials)
- Search and match songs with smart selection
- Generate QR codes linking to Spotify
- Export print-ready PDF cards (2x5 grid per A4 page)

**Web Interface (v2.0):**
- Drag-drop CSV upload with real-time validation
- Visual Spotify matching with confidence scores and album art
- Interactive card carousel with 3D flip preview
- Five color palettes + per-genre customization
- Card curation (include/exclude individual cards)
- PDF export with optional cutting lines

---

<details>
<summary>Original Vision (v1.0 - Archived for reference)</summary>

## Vision

A .NET 10 console application that generates printable PDF cards for a custom Hitster-style music guessing game. The app provides a wizard-like experience using Spectre.Console, guiding users through importing their song collection from CSV, enriching it with Spotify data, and producing professional print-ready cards.

The user wants to create their own Hitster game with a personal music collection. Each card features a QR code on the front (linking to Spotify) and the song's year, artist, title, and genre on the back. The app handles the entire workflow: data import, Spotify lookup, QR generation, and PDF export.

## Problem

Creating custom Hitster-style cards manually is tedious. You need to:
- Find Spotify track IDs for each song
- Generate QR codes pointing to those tracks
- Design and layout cards with year/artist/title/genre
- Arrange them for printing with proper cutting guides

This app automates the entire process, turning a CSV of songs into print-ready PDF cards.

## Success Criteria

How we know this worked:

- [x] User can import a semicolon-separated CSV with title, artist, year, genre headers
- [x] Validation catches malformed CSV and unknown genres with clear error messages
- [x] Spotify credentials are validated against the API before proceeding
- [x] Each song is matched to a Spotify track ID using smart selection logic
- [x] QR codes are generated linking to open.spotify.com/track/{ID}
- [x] Cards are credit-card sized (~85x55mm), 10 per A4 page in 2x5 grid
- [x] PDF has separate pages for fronts and backs (double-sided printing support)
- [x] Genre icons and optional background colors work correctly
- [x] The entire flow works on Windows, Mac, and Linux

## Scope

### Building
- Spectre.Console wizard UI with FIGlet header and two-panel layout
- CSV import with semicolon separator and header validation
- Genre validation against 30+ popular genres + 5 French genres
- Spotify API authentication with client credentials flow
- Spotify track search with smart original-track selection
- Interactive fallback when automatic track selection fails
- QR code generation for Spotify URLs
- Card design: front (QR code), back (year large/centered, artist above, title below, genre icon)
- Optional genre-based background colors (80% transparency)
- PDF export with 10 cards per A4, cutting line indicators
- Fronts and backs on separate pages for double-sided printing

### Not Building
- Persistence between runs (start fresh each time)
- Batch retry on API failure (restart from beginning)
- Performance optimization (prefer readable code)
- Complex error recovery (fail fast, restart)
- Database or file caching of Spotify lookups

## Context

This is a greenfield project. The directory has a git repo initialized but no code yet.

**UI Flow (8 steps):**
1. Ask for CSV file path
2. Validate CSV content (fail fast on first error)
3. Ask for Spotify API credentials (client ID + secret)
4. Search Spotify for each song, select best match, handle ambiguity
5. Generate QR codes for each track
6. Ask if user wants background colors
7. Generate card designs (front + back)
8. Export to single PDF with 10 cards per A4

**Spectre.Console Layout:**
- Top: FIGlet title "Hitster Card Generator"
- Below: Two columns - left shows step menu (auto-progressing), right shows current step content

## Constraints

- **Tech stack**: .NET 10 console application
- **UI**: Spectre.Console for all user interaction
- **Cross-platform**: Must work on Windows, Mac, Linux
- **Code style**: Prefer simplicity and readability over performance
- **Libraries**:
  - Spectre.Console (UI)
  - QuestPDF (PDF generation - modern, fluent API)
  - QRCoder (QR code generation)
  - CsvHelper or manual parsing for CSV

## Decisions Made

| Decision | Choice | Rationale |
|----------|--------|-----------|
| CSV separator | Semicolon | Song titles may contain commas |
| Card size | 85x55mm (credit card) | Standard Hitster card size, 10 fit on A4 |
| PDF layout | Separate front/back pages | Easy double-sided printing |
| Spotify URL | open.spotify.com/track/{ID} | Official Spotify web/app URL format |
| Genre count | 30 popular + 5 French | Covers most music, includes French chansons/pop |
| PDF library | QuestPDF | Modern, fluent API, good for complex layouts |
| QR library | QRCoder | Simple, well-maintained, cross-platform |

## Open Questions

Things to figure out during execution:

- [x] Exact list of 30 popular genres + colors + icons
- [x] Icon set to use for genres (Unicode emoji)
- [x] Spotify search ranking algorithm details (album_type, release_date, remastered filters)

</details>

---
*Initialized: 2025-12-20*
*v1.0 Shipped: 2025-12-21*
*v2.0 Shipped: 2025-12-21*
*v2.1 Shipped: 2025-12-22*
