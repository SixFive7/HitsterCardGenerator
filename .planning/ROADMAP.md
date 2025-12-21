# Roadmap: Hitster Card Generator

## Overview

Transform a CSV of songs into print-ready Hitster-style cards through a wizard-like console experience. Starting with project foundation and Spectre.Console UI, we add CSV import with validation, integrate Spotify for track lookups, and finish with QR code generation and PDF card export.

## Domain Expertise

None

## Phases

**Phase Numbering:**
- Integer phases (1, 2, 3): Planned milestone work
- Decimal phases (2.1, 2.2): Urgent insertions (marked with INSERTED)

- [x] **Phase 1: Foundation** - Project setup with Spectre.Console UI skeleton
- [x] **Phase 2: CSV Import** - File input, parsing, and validation
- [x] **Phase 3: Spotify Integration** - API authentication and track search
- [x] **Phase 4: Card Generation** - QR codes, card design, and PDF export

## Phase Details

### Phase 1: Foundation
**Goal**: Running .NET 10 console app with Spectre.Console UI structure (FIGlet header, two-panel layout with step menu)
**Depends on**: Nothing (first phase)
**Research**: Unlikely (established .NET patterns, well-documented Spectre.Console)
**Plans**: 1 (01-01-PLAN.md)

### Phase 2: CSV Import
**Goal**: Import and validate semicolon-separated CSV with title, artist, year, genre headers; validate genres against 30+ popular genres + 5 French genres
**Depends on**: Phase 1
**Research**: Unlikely (standard CSV parsing, internal genre validation)
**Plans**: 2 (02-01-PLAN.md, 02-02-PLAN.md)

### Phase 3: Spotify Integration
**Goal**: Authenticate with Spotify API, search for tracks, apply smart selection logic, provide interactive fallback for ambiguous matches
**Depends on**: Phase 2
**Research**: Likely (external API integration)
**Research topics**: Spotify Web API client credentials flow, search endpoint parameters, track metadata fields, rate limiting
**Plans**: 2 (03-01-PLAN.md, 03-02-PLAN.md)

### Phase 4: Card Generation
**Goal**: Generate QR codes linking to Spotify, design credit-card sized cards (85x55mm) with front (QR) and back (year/artist/title/genre icon), export PDF with cards in grid layout and cutting guides
**Depends on**: Phase 3
**Research**: Likely (PDF generation, QR library)
**Research topics**: QuestPDF fluent API for card layouts, QRCoder usage, image embedding, page layout for grid
**Plans**: 3 (04-01-PLAN.md, 04-02-PLAN.md, 04-03-PLAN.md)
**Note**: Card layout adjusted to 2x5 grid (10 per A4) - 4x4 (16) doesn't physically fit with 85x55mm cards

## Progress

**Execution Order:**
Phases execute in numeric order: 1 → 2 → 3 → 4

| Phase | Plans Complete | Status | Completed |
|-------|----------------|--------|-----------|
| 1. Foundation | 1/1 | Complete | 2025-12-20 |
| 2. CSV Import | 2/2 | Complete | 2025-12-21 |
| 3. Spotify Integration | 2/2 | Complete | 2025-12-21 |
| 4. Card Generation | 3/3 | Complete | 2025-12-21 |

**MILESTONE COMPLETE** - All 8 plans across 4 phases executed successfully.
