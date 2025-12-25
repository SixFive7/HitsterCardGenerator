# Phase 20: Remove CI/CD - Context

**Gathered:** 2025-12-25
**Status:** Ready for planning

<vision>
## How This Should Work

Clean slate approach — delete the CI/CD workflow entirely. Users clone the repo and build locally with `docker compose build`. No registry dependency, no GitHub Actions complexity. Just source code and a simple build command.

</vision>

<essential>
## What Must Be Nailed

- **Clear .env setup** - Users immediately know what secrets are needed (Spotify credentials) with a proper .env.example file
- **No traces of old CI/CD** - Clean removal with no orphaned references or confusion in the codebase

</essential>

<boundaries>
## What's Out of Scope

- README updates (that's Phase 21)
- Keep it simple: delete workflow + add .env.example

</boundaries>

<specifics>
## Specific Ideas

- .env.example should include both required AND optional variables:
  - Required: SPOTIFY_CLIENT_ID, SPOTIFY_CLIENT_SECRET (with placeholder values)
  - Optional: PUID, PGID, TZ (matching docker-compose.yml environment section)

</specifics>

<notes>
## Additional Context

Part of v2.5 Self-Hosting milestone. The goal is removing dependency on GHCR and CI/CD so users can self-host by simply cloning from any git source (GitHub, GitLab, etc.) and building locally.

</notes>

---

*Phase: 20-remove-ci-cd*
*Context gathered: 2025-12-25*
