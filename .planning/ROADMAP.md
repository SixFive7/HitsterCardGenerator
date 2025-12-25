# Roadmap: Hitster Card Generator

## Milestones

- ✅ [v1.0 MVP](milestones/v1.0-ROADMAP.md) (Phases 1-4) - SHIPPED 2025-12-21
- ✅ [v2.0 Web Interface](milestones/v2.0-ROADMAP.md) (Phases 5-9) - SHIPPED 2025-12-21
- ✅ [v2.1 Enhancements](milestones/v2.1-ROADMAP.md) (Phases 10-11) - SHIPPED 2025-12-22
- ✅ [v2.2 Polish](milestones/v2.2-ROADMAP.md) (Phase 12) - SHIPPED 2025-12-22
- ✅ [v2.3 Containerization](milestones/v2.3-ROADMAP.md) (Phases 13-16) - SHIPPED 2025-12-24
- ✅ [v2.4 Features](milestones/v2.4-ROADMAP.md) (Phases 17-19) - SHIPPED 2025-12-24
- 🚧 **v2.5 Self-Hosting** - Phases 20-21 (in progress)

## Current Milestone

### 🚧 v2.5 Self-Hosting (In Progress)

**Milestone Goal:** Remove CI/CD pipeline and registry dependency - users build locally from any git source.

#### Phase 20: Remove CI/CD

**Goal**: Delete release.yml workflow and create .env.example for clearer setup
**Depends on**: Previous milestone complete
**Research**: Unlikely (file deletion + simple config)
**Plans**: TBD

Plans:
- [ ] 20-01: TBD (run /gsd:plan-phase 20 to break down)

#### Phase 21: Local Build Docs

**Goal**: Update README for clone + docker compose build workflow
**Depends on**: Phase 20
**Research**: Unlikely (documentation only)
**Plans**: TBD

Plans:
- [ ] 21-01: TBD (run /gsd:plan-phase 21 to break down)

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

<details>
<summary>✅ v2.2 Polish (Phase 12) - SHIPPED 2025-12-22</summary>

Add devcontainer for Claude Code sandbox-free development.

- [x] **Phase 12: Devcontainer Setup** - Devcontainer with Claude Code feature (1 plan)

**Total:** 1 phase, 1 plan

See [milestones/v2.2-ROADMAP.md](milestones/v2.2-ROADMAP.md) for full details.

</details>

<details>
<summary>✅ v2.3 Containerization (Phases 13-16) - SHIPPED 2025-12-24</summary>

Docker containerization with automated CI/CD pipeline for easy deployment via docker-compose.

- [x] **Phase 13: Dev Experience** - F5 compound launch with Release-only NpmBuild (1 plan)
- [x] **Phase 14: Docker Image** - Multi-stage Dockerfile with non-root user (1 plan)
- [x] **Phase 15: GitHub Actions CI** - GHCR publishing + CHANGELOG-based release notes (1 plan)
- [x] **Phase 16: Multi-Arch & Docs** - AMD64 + ARM64 builds, README with screenshots (2 plans)

**Total:** 4 phases, 5 plans

See [milestones/v2.3-ROADMAP.md](milestones/v2.3-ROADMAP.md) for full details.

</details>

<details>
<summary>✅ v2.4 Features (Phases 17-19) - SHIPPED 2025-12-24</summary>

Add playlist builder as alternative to CSV upload - search Spotify and build a playlist directly in the app.

- [x] **Phase 17: Spotify Search** - Backend search endpoint + search UI component (1 plan)
- [x] **Phase 18: Playlist Builder** - Playlist store + PlaylistBuilder component (1 plan)
- [x] **Phase 19: Flow Integration** - Landing page dual-path, skip matching for playlists (1 plan)

**Total:** 3 phases, 3 plans

See [milestones/v2.4-ROADMAP.md](milestones/v2.4-ROADMAP.md) for full details.

</details>

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
| 13. Dev Experience | v2.3 | 1/1 | Complete | 2025-12-23 |
| 14. Docker Image | v2.3 | 1/1 | Complete | 2025-12-24 |
| 15. GitHub Actions CI | v2.3 | 1/1 | Complete | 2025-12-24 |
| 16. Multi-Arch & Docs | v2.3 | 2/2 | Complete | 2025-12-24 |
| 17. Spotify Search | v2.4 | 1/1 | Complete | 2025-12-24 |
| 18. Playlist Builder | v2.4 | 1/1 | Complete | 2025-12-24 |
| 19. Flow Integration | v2.4 | 1/1 | Complete | 2025-12-24 |
| 20. Remove CI/CD | v2.5 | 0/? | Not started | - |
| 21. Local Build Docs | v2.5 | 0/? | Not started | - |

**v1.0 Complete** — **v2.0 Complete** — **v2.1 Complete** — **v2.2 Complete** — **v2.3 Complete** — **v2.4 Complete**
