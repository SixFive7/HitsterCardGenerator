# Roadmap: Hitster Card Generator

## Milestones

- ✅ [v1.0 MVP](milestones/v1.0-ROADMAP.md) (Phases 1-4) - SHIPPED 2025-12-21
- ✅ [v2.0 Web Interface](milestones/v2.0-ROADMAP.md) (Phases 5-9) - SHIPPED 2025-12-21
- ✅ [v2.1 Enhancements](milestones/v2.1-ROADMAP.md) (Phases 10-11) - SHIPPED 2025-12-22
- ✅ **v2.2 Polish** (Phase 12) - SHIPPED 2025-12-22

## Completed Milestones

<details>
<summary>✅ v1.0 MVP (Phases 1-4) - SHIPPED 2025-12-21</summary>

Transform a CSV of songs into print-ready Hitster-style cards through a wizard-like console experience.

- [x] **Phase 1: Foundation** - Project setup with Spectre.Console UI skeleton (1 plan)
- [x] **Phase 2: CSV Import** - File input, parsing, and validation (2 plans)
- [x] **Phase 3: Spotify Integration** - API authentication and track search (2 plans)
- [x] **Phase 4: Card Generation** - QR codes, card design, and PDF export (3 plans)

**Total:** 4 phases, 8 plans

See [milestones/v1.0-ROADMAP.md](milestones/v1.0-ROADMAP.md) for full details.

</details>

<details>
<summary>✅ v2.0 Web Interface (Phases 5-9) - SHIPPED 2025-12-21</summary>

Transform the console application into a modern web interface using .NET Minimal API backend with Svelte + Tailwind frontend.

- [x] **Phase 5: Web Foundation** - Minimal API + Svelte/Tailwind setup (2 plans)
- [x] **Phase 6: File Upload** - Drag-drop CSV with validation display (1 plan)
- [x] **Phase 7: Spotify Web Flow** - Match API with confidence scoring (2 plans)
- [x] **Phase 8: Card Preview** - Carousel with color palettes and curation (2 plans)
- [x] **Phase 9: PDF Export** - Server-side generation with cutting lines (2 plans)

**Total:** 5 phases, 9 plans

See [milestones/v2.0-ROADMAP.md](milestones/v2.0-ROADMAP.md) for full details.

</details>

<details>
<summary>✅ v2.1 Enhancements (Phases 10-11) - SHIPPED 2025-12-22</summary>

Simplify startup by having .NET serve the Svelte frontend directly - single process instead of running backend and frontend separately.

- [x] **Phase 10: Static File Serving** - .NET serves Svelte build from wwwroot (1 plan)
- [x] **Phase 11: Build Integration** - MSBuild targets automate npm install/build (1 plan)

**Total:** 2 phases, 2 plans

See [milestones/v2.1-ROADMAP.md](milestones/v2.1-ROADMAP.md) for full details.

</details>

## ✅ v2.2 Polish (Complete)

**Milestone Goal:** Add devcontainer for Claude Code sandbox-free development

### Phase 12: Devcontainer Setup

**Goal**: Configure devcontainer with Claude Code feature for unconstrained development
**Depends on**: Phase 11
**Research**: Complete (anthropics devcontainer-features)
**Plans**: 1

Plans:
- [x] 12-01: Devcontainer with Claude Code feature

## Progress

| Phase | Milestone | Plans Complete | Status | Completed |
|-------|-----------|----------------|--------|-----------|
| 1. Foundation | v1.0 | 1/1 | Complete | 2025-12-20 |
| 2. CSV Import | v1.0 | 2/2 | Complete | 2025-12-21 |
| 3. Spotify Integration | v1.0 | 2/2 | Complete | 2025-12-21 |
| 4. Card Generation | v1.0 | 3/3 | Complete | 2025-12-21 |
| 5. Web Foundation | v2.0 | 2/2 | Complete | 2025-12-21 |
| 6. File Upload | v2.0 | 1/1 | Complete | 2025-12-21 |
| 7. Spotify Web Flow | v2.0 | 2/2 | Complete | 2025-12-21 |
| 8. Card Preview | v2.0 | 2/2 | Complete | 2025-12-21 |
| 9. PDF Export | v2.0 | 2/2 | Complete | 2025-12-21 |
| 10. Static File Serving | v2.1 | 1/1 | Complete | 2025-12-22 |
| 11. Build Integration | v2.1 | 1/1 | Complete | 2025-12-22 |
| 12. Devcontainer Setup | v2.2 | 1/1 | Complete | 2025-12-22 |

**v1.0 Complete** — **v2.0 Complete** — **v2.1 Complete** — **v2.2 Complete**
