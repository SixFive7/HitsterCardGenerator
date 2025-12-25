# Phase 21: Local Build Docs - Context

**Gathered:** 2025-12-25
**Status:** Ready for planning

<vision>
## How This Should Work

Two simple docker-compose snippets in the README that users can copy-paste directly into Portainer (or any compose manager) and it just works. No git clone required — Docker builds directly from the GitHub URL.

The first compose is the simple version: just the app with inline environment variables for Spotify credentials and port 8080 exposed.

The second compose is for users with Traefik already running: same as the first but with Traefik labels added for routing (e.g., `Host(\`hitster.huisman.io\`)`). No network configuration — users who run Traefik know their own setup.

The whole point is zero friction for self-hosters. Copy, paste, fill in Spotify credentials, done.

</vision>

<essential>
## What Must Be Nailed

- **Portainer-friendly** — Copy-paste into Portainer stacks UI works without modification (except credentials)
- **Build from URL** — Docker builds directly from GitHub URL, no local clone required
- **Inline credentials** — Environment vars directly in the compose YAML, not referencing external .env files

</essential>

<boundaries>
## What's Out of Scope

- Troubleshooting sections — keep it clean, no "if this doesn't work" content
- Volume mounts — PDFs download through browser, no persistence needed
- Traefik network config — just the labels, users configure their own networks
- Detailed Spotify credential instructions — just link to Spotify Developer Dashboard

</boundaries>

<specifics>
## Specific Ideas

- Image name: `hitster-card-generator`
- Port mapping: `8080:8080`
- Traefik labels format:
  ```yaml
  labels:
    traefik.enable: true
    traefik.http.routers.hitster.rule: Host(`hitster.huisman.io`)
    traefik.http.services.hitster.loadbalancer.server.port: 8080
  ```
- Replace existing Docker section in README (don't add new section)
- Minimal cleanup of existing README content — just fix what's broken by CI/CD removal

</specifics>

<notes>
## Additional Context

This is the companion to Phase 20 (Remove CI/CD). Now that there's no pre-built image in a registry, users need a way to build locally. But "locally" here means "Docker builds from GitHub URL" — the user's machine never needs the source code directly.

The goal is maintaining the "just works" experience even without CI/CD infrastructure.

</notes>

---

*Phase: 21-local-build-docs*
*Context gathered: 2025-12-25*
