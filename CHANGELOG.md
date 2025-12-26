# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.6.0] - 2025-12-25

v2.6 adds visual branding with a colorful rainbow gradient theme for the favicon and landing page.

### Added
- Rainbow gradient favicon replacing the plain black music note
- Animated rainbow gradient music note SVG on landing page

## [2.5.0] - 2025-12-25

v2.5 focuses on self-hosting simplicity by removing CI/CD dependencies and providing clear local build documentation.

### Added
- `.env.example` with documented Spotify API configuration
- Docker Compose examples for building from GitHub URL directly

### Removed
- GitHub Actions CI/CD pipeline (simplified to local builds)

### Changed
- README updated with comprehensive self-hosting instructions

## [2.4.1] - 2025-12-25

Patch release adding container permission controls for NAS and shared hosting environments.

### Added
- PUID/PGID environment variables for custom user/group IDs in Docker
- TZ environment variable for timezone configuration

## [2.4.0] - 2025-12-24

v2.4 introduces Spotify search and playlist building, allowing users to create card sets by searching for songs directly instead of uploading a CSV.

### Added
- Spotify search endpoint with track search API
- SpotifySearch component with real-time search results
- Playlist builder with add/remove functionality
- Genre assignment for playlist tracks
- Landing page with dual-path choice (CSV upload or playlist builder)
- Direct flow from playlist builder to card preview

### Fixed
- Vite dev server now binds to all interfaces for proper port forwarding

## [2.3.0] - 2025-12-24

v2.3 completes the Containerization milestone, enabling deployment via Docker and automated releases through GitHub Actions.

### Added
- Docker image with multi-stage build for easy deployment
- GitHub Actions CI for automated releases to GHCR on tag push
- Health check endpoint at `/api/health`
- VS Code compound launch configuration for F5 debugging
- Devcontainer for Claude Code development

### Changed
- Application port changed from 5000 to 8080 (non-root container compatibility)
- Frontend build only runs in Release mode (faster Debug builds)

## [2.2.0] - 2025-12-22

v2.2 adds development container support for consistent development environments with Claude Code.

### Added
- Devcontainer configuration for VS Code
- Claude Code authentication persistence across container rebuilds

## [2.1.0] - 2025-12-22

v2.1 consolidates the application into a single process by embedding the Svelte frontend directly in the .NET backend.

### Added
- Static file serving from .NET (serves Svelte build output)
- MSBuild integration for frontend build during publish

### Changed
- Eliminated need for separate frontend dev server in production

## [2.0.0] - 2025-12-21

v2.0 transforms the application from a console wizard to a modern web interface, enabling visual card preview and customization before PDF export.

### Added
- Web interface with Svelte + Tailwind frontend
- Drag-drop CSV upload with real-time validation
- Interactive card preview carousel with flip animations
- Genre color customization with preset palettes
- Card curation (include/exclude individual cards)
- PDF export with optional cutting lines

### Changed
- Migrated from console wizard to web application
- Card layout: 2x5 grid (10 per A4 page) for proper 85x55mm sizing

## [1.0.0] - 2025-12-21

Initial release of Hitster Card Generator.

### Added
- CSV import with validation (semicolon-separated: title;artist;year;genre)
- Spotify track matching with smart selection (prioritizes albums over singles)
- QR code generation linking directly to Spotify tracks
- PDF export with credit-card sized cards (85x55mm)
- 30+ genre validation including 5 French genres
- Console wizard interface with Spectre.Console
