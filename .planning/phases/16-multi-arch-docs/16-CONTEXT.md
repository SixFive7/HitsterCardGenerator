# Phase 16: Multi-Architecture & User Docs - Context

**Gathered:** 2025-12-24
**Status:** Ready for planning

<vision>
## How This Should Work

The focus is on user experience over technical plumbing. When someone discovers this project, they should be able to get it running quickly with minimal friction.

The README is the star of this phase - a comprehensive, friendly document that serves three audiences:
1. **End users** - How to use the app to create Hitster cards
2. **Admins** - How to deploy and configure hosting
3. **Developers** - How the tech stack works and how to contribute locally

Structure: Quick start at the top (get running fast), then details below for those who want depth. Friendly casual tone - like explaining to a friend, not corporate documentation.

Both docker-compose examples live directly in the README - no separate files. Users copy the example they need. The simple version is copy-paste ready with minimal changes needed. The Traefik version follows the pattern in `example.yaml` (root directory reference file, not to be committed).

</vision>

<essential>
## What Must Be Nailed

- **Two compose patterns** - Both simple (direct ports) and Traefik versions, well-commented, embedded in README
- **ARM64 + AMD64 builds** - Standard linux/amd64 and linux/arm64 via buildx
- **Full workflow screenshots** - Show Upload → Match → Preview → Export journey
- **Rainbow music note logo** - Colorful, musical branding at the top
- **Standard GitHub badges** - Build status, license, Docker pulls

</essential>

<boundaries>
## What's Out of Scope

- No Kubernetes or other orchestrators - just docker-compose
- No SSL/TLS configuration - Traefik handles that at proxy level
- No separate docker-compose files - examples embedded in README
- No CONTRIBUTING.md - personal project, not expecting contributors
- No step-by-step Spotify setup guide - just mention the required env vars

</boundaries>

<specifics>
## Specific Ideas

- **README structure:** Logo + badges at top → Quick start → Screenshots → Deployment (simple + Traefik examples) → Environment variables → Developer section (architecture + local dev)
- **Traefik example:** Derive from `example.yaml` in project root (add to .gitignore)
- **License:** MIT
- **Dev section:** Both architecture overview AND local development instructions
- **CLAUDE.md update:** Add instruction to always keep README in sync with project changes

</specifics>

<notes>
## Additional Context

The README should feel polished and welcoming. Screenshots are essential to show what the app looks like before users commit to setting it up.

Multi-arch builds are technical plumbing - important but straightforward. The real value-add is making deployment and understanding the project effortless for all three audiences.

</notes>

---

*Phase: 16-multi-arch-docs*
*Context gathered: 2025-12-24*
