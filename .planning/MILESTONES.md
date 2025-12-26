# Project Milestones: Hitster Card Generator

## v2.9 SkiaSharp Rendering (Shipped: 2025-12-26)

**Delivered:** Unified card rendering with SkiaSharp as single source of truth for both web preview and PDF export.

**Phases completed:** 31 (1 plan total)

**Key accomplishments:**

- Created CardRenderer.cs with SkiaSharp rendering using modern SKFont API (zero deprecation warnings)
- Updated SkiaSharp from 3.116.1 to 3.119.1
- Simplified CardDesigner.cs to delegate to CardRenderer
- Updated PdfExporter.cs to embed pre-rendered PNGs instead of re-rendering

**Stats:**

- 4 files modified
- ~200 lines removed (code consolidation)
- 1 phase, 1 plan, 5 tasks
- Same day (2025-12-26)

**Git range:** `feat(31-01)` -> `v2.9`

**What's next:** All rendering unified through CardRenderer. Ready for next milestone.

---

## v2.8 Simplification (Shipped: 2025-12-26)

**Delivered:** Unified card rendering between preview and PDF, hardcoded color scheme, card redesign, and E2E testing.

**Phases completed:** 26-30 (7 plans total)

**Key accomplishments:**

- QuestPDF server-side rendering for pixel-perfect preview/PDF parity
- Removed palette selection, hardcoded Spotify green palette
- Card redesign: centered QR code front, album art + metadata back
- Automated E2E testing with Chrome DevTools MCP

**Stats:**

- 5 phases, 7 plans
- Duration: 1 day (2025-12-26)

**Git range:** See [v2.8-ROADMAP.md](milestones/v2.8-ROADMAP.md) for details

**What's next:** v2.9 migrated to SkiaSharp rendering.

---

## v2.7 Fixes (Shipped: 2025-12-26)

**Delivered:** Bug fixes and stability improvements - remove obsolete inclusion feature, fix card flip, fix color palettes.

**Phases completed:** 23-25 (3 plans total)

**Key accomplishments:**

- Removed obsolete inclusion feature that was never fully implemented
- Fixed card flip button that wasn't responding to clicks (MouseEvent passed as card index)
- Fixed color palettes not updating card previews (Svelte 5 reactivity issue with derived stores)

**Stats:**

- 23 files modified
- +1,114/-231 lines
- 3 phases, 3 plans, 8 tasks
- Same day (2025-12-26, ~14 min)

**Git range:** `fix: remove inclusion feature` → `fix: add reactive binding for color palettes`

**What's next:** All planned milestones complete. Ready for next milestone.

---

## v2.6 Improvements (Shipped: 2025-12-25)

**Delivered:** Rainbow gradient favicon and landing page music note SVG replacing default Vite icon and emoji.

**Phases completed:** 22 (1 plan total)

**Key accomplishments:**

- Created rainbow gradient favicon with beamed eighth notes design
- Replaced emoji music note with inline SVG using matching rainbow gradient
- Updated page title from "web" to "Hitster Card Generator"

**Stats:**

- 8 files modified
- +261/-26 lines
- 1 phase, 1 plan, 3 tasks
- Same day (2025-12-25, 2 min)

**Git range:** `feat(22-01)`

**What's next:** Project fully branded with custom visual identity. Ready for next milestone.

---

## v2.5 Self-Hosting (Shipped: 2025-12-25)

**Delivered:** Remove CI/CD pipeline and registry dependency - users build locally from any git source with zero-friction docker-compose.

**Phases completed:** 20-21 (2 plans total)

**Key accomplishments:**

- Deleted GitHub Actions CI/CD workflow and .github/ directory structure
- Created .env.example with documented Spotify and container environment variables
- Added Portainer-friendly build-from-URL docker-compose in Quick Start
- Added Traefik reverse proxy example with build-from-URL approach
- Updated documentation for zero-registry self-hosting workflow

**Stats:**

- 11 files modified
- +542/-151 lines
- 2 phases, 2 plans, 4 tasks
- Same day (2025-12-25, 47 min)

**Git range:** `feat(20-01)` → `feat(21-01)`

**What's next:** Project complete with full self-hosting support. Users can deploy from any git source without container registry access.

---

## v2.4 Features (Shipped: 2025-12-24)

**Delivered:** Playlist builder as alternative to CSV upload - search Spotify and build a playlist directly in the app.

**Phases completed:** 17-19 (3 plans total)

**Key accomplishments:**

- Spotify search API endpoint with debounced SpotifySearch component
- Playlist store with add/remove/genre functionality and visual curation
- PlaylistBuilder component with genre dropdown and track management
- Landing page dual-path choice (Upload CSV vs Build Playlist)
- Playlist-to-preview direct flow that skips Spotify matching

**Stats:**

- 21 files created/modified
- +1,722 lines
- 3 phases, 3 plans, 6 tasks
- 1 day (2025-12-24, 16 min active dev)

**Git range:** `feat(17-01)` → `feat(19-01)`

**What's next:** All planned milestones complete. Project offers two paths: CSV upload with Spotify matching, or build playlist directly.

---

## v2.3 Containerization (Shipped: 2025-12-24)

**Delivered:** Docker containerization with automated CI/CD pipeline for easy deployment via docker-compose.

**Phases completed:** 13-16 (5 plans total)

**Key accomplishments:**

- F5 compound launch with Release-only NpmBuild for fast debug builds
- Multi-stage Dockerfile with non-root user on port 8080 and wget health check
- GitHub Actions release workflow with GHCR publishing + CHANGELOG.md
- Multi-arch Docker builds (AMD64 + ARM64) with QEMU emulation
- Comprehensive README with workflow screenshots and docker-compose examples
- MIT LICENSE and CLAUDE.md project guide

**Stats:**

- 29 files created/modified
- +2,576/-40 lines
- 4 phases, 5 plans, 13 tasks
- 2 days (2025-12-23 → 2025-12-24)

**Git range:** `feat(13-01)` → `feat(16-02)`

**What's next:** All planned milestones complete. Project is production-ready with Docker containerization, CI/CD, and multi-arch support.

---

## v2.2 Polish (Shipped: 2025-12-22)

**Delivered:** Devcontainer configuration with Claude Code for sandbox-free development in containerized environment.

**Phases completed:** 12 (1 plan total)

**Key accomplishments:**

- Configured devcontainer with .NET 10 + Node.js 20 + Claude Code
- Volume mount for Claude config persistence across rebuilds
- VS Code extensions for C#, Svelte, Tailwind CSS
- Port forwarding for API (5657) and Vite (5173)

**Stats:**

- 7 files created/modified
- +332/-70 lines
- 1 phase, 1 plan, 2 tasks
- 1 day (2025-12-22)

**Git range:** `366bfd4` → `8962386`

**What's next:** All planned milestones complete. Project is production-ready with full devcontainer support.

---

## v2.1 Enhancements (Shipped: 2025-12-22)

**Delivered:** Single-process startup - `dotnet run` builds and serves complete application without needing separate frontend dev server.

**Phases completed:** 10-11 (2 plans total)

**Key accomplishments:**

- Static file middleware serves Svelte build from wwwroot
- SPA fallback routing with MapFallbackToFile("index.html")
- MSBuild NpmInstall target auto-runs npm install on first build
- MSBuild NpmBuild target compiles frontend before .NET build
- Vite configured to output directly to wwwroot
- Cross-platform support with Windows npm.cmd detection

**Stats:**

- 14 files created/modified
- +462/-19 lines
- 2 phases, 2 plans, 6 tasks
- 2 days (Dec 20-22, 2025)

**Git range:** `feat(10-01)` → `feat(11-01)`

**What's next:** All planned milestones complete. Project is production-ready with streamlined developer experience.

---

## v2.0 Web Interface (Shipped: 2025-12-21)

**Delivered:** Modern web interface with drag-drop CSV upload, visual Spotify matching, interactive card preview carousel, and PDF export with cutting lines.

**Phases completed:** 5-9 (9 plans total)

**Key accomplishments:**

- .NET Minimal API backend with modular endpoint structure
- Svelte 5 + Tailwind v4 frontend with animated landing page
- Drag-drop CSV upload with real-time validation display
- Spotify matching API with confidence scoring and album art display
- Interactive card carousel with CSS 3D flip animation
- Five preset color palettes + per-genre customization
- Card curation system (include/exclude individual cards)
- PDF export with SVG-based cutting lines toggle

**Stats:**

- 69 files created/modified
- +8,191 lines (C# + Svelte/TypeScript)
- 5 phases, 9 plans
- 1 day execution (~3.7 hours active development)

**Git range:** `feat(05-01)` → `feat(09-02)`

**What's next:** Both console and web interfaces complete. Future enhancements could include user authentication, saved sessions, or batch processing.

---

## v1.0 MVP (Shipped: 2025-12-21)

**Delivered:** Complete wizard-based console application that transforms a CSV of songs into print-ready PDF cards for a custom Hitster-style music guessing game.

**Phases completed:** 1-4 (8 plans total)

**Key accomplishments:**

- Spectre.Console wizard UI with FIGlet header and two-panel step-based layout
- CSV import with semicolon parsing and 35-genre validation with typo suggestions
- Spotify API integration with client credentials auth and smart track matching
- Interactive fallback for ambiguous Spotify matches (album > single > compilation priority)
- QR code generation linking directly to Spotify tracks
- Optional genre-based background colors (35 distinct colors)
- PDF export with 2x5 card grid per A4 page, mirrored backs for duplex printing

**Stats:**

- 48 files created/modified
- 2,404 lines of C#
- 4 phases, 8 plans
- 2 days from start to ship (2025-12-20 → 2025-12-21)

**Git range:** `feat(01-01)` → `feat(04-03)`

**What's next:** Project complete - ready for use. Future enhancements could include batch retry, persistence, or additional export formats.

---
