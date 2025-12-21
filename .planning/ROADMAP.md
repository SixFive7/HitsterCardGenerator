# Roadmap: Hitster Card Generator

## Milestones

- [v1.0 MVP](milestones/v1.0-ROADMAP.md) (Phases 1-4) - SHIPPED 2025-12-21
- **v2.0 Web Interface** (Phases 5-9) - IN PROGRESS

## Completed Milestones

<details>
<summary>v1.0 MVP (Phases 1-4) - SHIPPED 2025-12-21</summary>

Transform a CSV of songs into print-ready Hitster-style cards through a wizard-like console experience.

- [x] **Phase 1: Foundation** - Project setup with Spectre.Console UI skeleton (1 plan)
- [x] **Phase 2: CSV Import** - File input, parsing, and validation (2 plans)
- [x] **Phase 3: Spotify Integration** - API authentication and track search (2 plans)
- [x] **Phase 4: Card Generation** - QR codes, card design, and PDF export (3 plans)

**Total:** 4 phases, 8 plans

See [milestones/v1.0-ROADMAP.md](milestones/v1.0-ROADMAP.md) for full details.

</details>

## 🚧 v2.0 Web Interface (In Progress)

**Milestone Goal:** Transform the console application into a modern web interface using .NET Minimal API backend with Svelte + Tailwind frontend.

### Phase 5: Web Foundation - COMPLETE

**Goal**: Set up Minimal API project, Svelte/Vite frontend with Tailwind, establish project structure
**Depends on**: v1.0 complete
**Research**: Likely (Svelte/Vite/.NET integration patterns)
**Research topics**: Vite proxy setup, Svelte project structure, Tailwind configuration
**Plans**: 2

Plans:
- [x] 05-01: Web SDK & Minimal API Foundation
- [x] 05-02: Svelte + Tailwind Frontend

### Phase 6: File Upload - COMPLETE

**Goal**: CSV upload via browser with drag-drop, validation display reusing existing parser
**Depends on**: Phase 5
**Research**: Unlikely (standard patterns)
**Plans**: 1

Plans:
- [x] 06-01: CSV Upload & Validation Display

### Phase 7: Spotify Web Flow

**Goal**: OAuth redirect flow for Spotify authentication, track search UI with results display
**Depends on**: Phase 6
**Research**: Likely (web OAuth differs from client credentials)
**Research topics**: Spotify Authorization Code flow, PKCE, redirect handling
**Plans**: TBD

Plans:
- [ ] 07-01: TBD

### Phase 8: Card Preview

**Goal**: In-browser card preview with live updates, color picker for genre backgrounds
**Depends on**: Phase 7
**Research**: Unlikely (internal patterns)
**Plans**: TBD

Plans:
- [ ] 08-01: TBD

### Phase 9: PDF Export

**Goal**: Server-side PDF generation with QuestPDF, file download endpoint
**Depends on**: Phase 8
**Research**: Unlikely (QuestPDF already integrated)
**Plans**: TBD

Plans:
- [ ] 09-01: TBD

## Progress

| Phase | Milestone | Plans Complete | Status | Completed |
|-------|-----------|----------------|--------|-----------|
| 1. Foundation | v1.0 | 1/1 | Complete | 2025-12-20 |
| 2. CSV Import | v1.0 | 2/2 | Complete | 2025-12-21 |
| 3. Spotify Integration | v1.0 | 2/2 | Complete | 2025-12-21 |
| 4. Card Generation | v1.0 | 3/3 | Complete | 2025-12-21 |
| 5. Web Foundation | v2.0 | 2/2 | Complete | 2025-12-21 |
| 6. File Upload | v2.0 | 1/1 | Complete | 2025-12-21 |
| 7. Spotify Web Flow | v2.0 | 0/? | Not started | - |
| 8. Card Preview | v2.0 | 0/? | Not started | - |
| 9. PDF Export | v2.0 | 0/? | Not started | - |

**v1.0 Complete** — Starting v2.0 Web Interface (5 phases)
