# Phase 14: Docker Image - Context

**Gathered:** 2025-12-23
**Status:** Ready for planning

<vision>
## How This Should Work

A single, production-ready Docker container that can be pulled and run with minimal configuration. The image should embody best practices — multi-stage build for efficiency, non-root user for security, proper health checks — without introducing multi-container complexity.

Users configure Spotify credentials via environment variables at runtime. When using docker-compose, they can use a `.env` file for convenience. No secrets baked into the image.

</vision>

<essential>
## What Must Be Nailed

- **Small, efficient image** - Multi-stage build that produces a minimal runtime image
- **Clean configuration** - Clear what env vars are needed (SPOTIFY_CLIENT_ID, SPOTIFY_CLIENT_SECRET) and how to set them
- **Security best practices** - Non-root user, no secrets baked in, proper file permissions

</essential>

<boundaries>
## What's Out of Scope

- CI/CD automation - that's Phase 15 (GitHub Actions)
- Multi-architecture builds (ARM64) - that's Phase 16
- Docker Compose example files - that's Phase 16
- This phase is just the Dockerfile itself

</boundaries>

<specifics>
## Specific Ideas

- Base image choice is flexible - use whatever works best for .NET 10
- Document the choice and reasoning (why this base image, why these stages)
- Environment variables: SPOTIFY_CLIENT_ID, SPOTIFY_CLIENT_SECRET at minimum

</specifics>

<notes>
## Additional Context

This is part of v2.3 Containerization milestone. The Dockerfile needs to work with the existing .NET 10 Minimal API backend that serves the Svelte frontend from wwwroot.

The build should leverage the MSBuild integration from Phase 11 (NpmBuild target that runs in Release mode).

</notes>

---

*Phase: 14-docker-image*
*Context gathered: 2025-12-23*
