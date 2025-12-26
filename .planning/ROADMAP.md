# Roadmap: Hitster Card Generator

## Milestones

- ✅ [v1.0 MVP](milestones/v1.0-ROADMAP.md) (Phases 1-4) - SHIPPED 2025-12-21
- ✅ [v2.0 Web Interface](milestones/v2.0-ROADMAP.md) (Phases 5-9) - SHIPPED 2025-12-21
- ✅ [v2.1 Enhancements](milestones/v2.1-ROADMAP.md) (Phases 10-11) - SHIPPED 2025-12-22
- ✅ [v2.2 Polish](milestones/v2.2-ROADMAP.md) (Phase 12) - SHIPPED 2025-12-22
- ✅ [v2.3 Containerization](milestones/v2.3-ROADMAP.md) (Phases 13-16) - SHIPPED 2025-12-24
- ✅ [v2.4 Features](milestones/v2.4-ROADMAP.md) (Phases 17-19) - SHIPPED 2025-12-24
- ✅ [v2.5 Self-Hosting](milestones/v2.5-ROADMAP.md) (Phases 20-21) - SHIPPED 2025-12-25
- ✅ [v2.6 Improvements](milestones/v2.6-ROADMAP.md) (Phase 22) - SHIPPED 2025-12-25
- ✅ [v2.7 Fixes](milestones/v2.7-ROADMAP.md) (Phases 23-25) - SHIPPED 2025-12-26
- 🔄 [v2.8 Simplification](milestones/v2.8-ROADMAP.md) (Phases 26-30) - IN PROGRESS

## Current Milestone

### v2.8 Simplification (Phases 26-30)

Simplify the application by unifying card rendering, hardcoding the color scheme, redesigning cards, and adding automated E2E testing.

- [x] **Phase 26: Unified Rendering** - Research and implement unified card rendering approach (2/2 plans done)
- [x] **Phase 27: Simplify Color System** - Remove palette selection, hardcode Spotify palette (1/1 plan done)
- [ ] **Phase 28: Card Redesign** - New front/back design with centered elements
- [ ] **Phase 29: Design Polish** - Visual iteration until cards look beautiful
- [ ] **Phase 30: Automated E2E Testing** - Full flow testing with Chrome DevTools MCP

**Total:** 5 phases

## Completed Milestones (Recent)

<details>
<summary>✅ v2.7 Fixes (Phases 23-25) - SHIPPED 2025-12-26</summary>

Bug fixes and stability improvements - remove obsolete inclusion feature, fix card flip, fix color palettes.

- [x] **Phase 23: Remove Inclusion Feature** - Remove "included" button from UI and all code paths (1 plan)
- [x] **Phase 24: Fix Card Flip** - Fix the flip button functionality (1 plan)
- [x] **Phase 25: Fix Color Palettes** - Make color palettes actually apply to cards (1 plan)

**Total:** 3 phases, 3 plans

</details>

<details>
<summary>✅ v2.6 Improvements (Phase 22) - SHIPPED 2025-12-25</summary>

Add branded favicon and update main page music note with rainbow gradient.

- [x] **Phase 22: Branding** - Gradient favicon + SVG music note (1 plan)

**Total:** 1 phase, 1 plan

</details>

## Completed Milestones

<details>
<summary>✅ v2.5 Self-Hosting (Phases 20-21) - SHIPPED 2025-12-25</summary>

Remove CI/CD pipeline and registry dependency - users build locally from any git source with zero-friction docker-compose.

- [x] **Phase 20: Remove CI/CD** - Delete release.yml workflow, create .env.example (1 plan)
- [x] **Phase 21: Local Build Docs** - README with build-from-URL docker-compose examples (1 plan)

**Total:** 2 phases, 2 plans

See [milestones/v2.5-ROADMAP.md](milestones/v2.5-ROADMAP.md) for full details.

</details>

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
| 20. Remove CI/CD | v2.5 | 1/1 | Complete | 2025-12-25 |
| 21. Local Build Docs | v2.5 | 1/1 | Complete | 2025-12-25 |
| 22. Branding | v2.6 | 1/1 | Complete | 2025-12-25 |
| 23. Remove Inclusion Feature | v2.7 | 1/1 | Complete | 2025-12-26 |
| 24. Fix Card Flip | v2.7 | 1/1 | Complete | 2025-12-26 |
| 25. Fix Color Palettes | v2.7 | 1/1 | Complete | 2025-12-26 |
| 26. Unified Rendering | v2.8 | 2/2 | Complete | 2025-12-26 |
| 27. Simplify Color System | v2.8 | 1/1 | Complete | 2025-12-26 |
