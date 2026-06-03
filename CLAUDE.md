# Claude Code Project Guide

This file provides guidance for Claude Code when working on the HitsterCardGenerator project.

## Project Overview

A .NET 10 web application that generates printable PDF cards for a custom Hitster-style music guessing game. Users upload a CSV of songs, the app matches them to Spotify tracks, and generates credit-card sized cards with QR codes linking to the songs.

## Tech Stack

- **Backend:** .NET 10 Minimal API
- **Frontend:** Svelte 5 + Tailwind CSS v4 (Vite)
- **Libraries:** QuestPDF (PDF generation), QRCoder (QR codes), SpotifyAPI.Web (Spotify integration)

## Development

- Run `dotnet run` for production (serves frontend from wwwroot)
- For HMR development: Use F5 compound launch in VS Code

## Documentation

When making any changes to the project ensure the README.md is updated to reflect those changes.

## Playwright usage policy

Four Playwright MCP servers are configured: `playwright-headless`, `playwright-interactive`, `playwright-tracing`, `playwright-persistent`. Apply this policy when choosing one:

1. **Default to `playwright-headless`** — parallel-safe, ephemeral, no state. Use it for almost everything that needs browser automation.
2. **Use Playwright as little as possible.** After one research round on a new site, evaluate whether the task can be done by calling the underlying API directly (with credentials you already have). Prefer direct HTTP over browser automation — it is faster, more reliable, and easier to debug.
3. **Use `playwright-tracing`** to reverse-engineer a site's API shapes (request/response, auth flow, WebSocket frames). The goal is usually to *stop* using Playwright on that site — capture enough trace to make direct calls work, then switch.
4. **Use `playwright-interactive` only** when a credentialed login is required and the credentials are not already on hand. Tell the user what you are about to do. Do not see, ask for, or attempt to capture credentials — the user types them directly in the visible window.
5. **Use `playwright-persistent` only** when the user explicitly instructs you to AND has validated the request. Saved logins in `playwright/persistent/profile/` are accessible to any agent that touches that server — treat as a security risk, never default to it.

The rules above are sufficient for using the servers. Only read [playwright/README.md](playwright/README.md) if you need the architecture rationale, the per-mode settings table, or are about to *change* the Playwright configuration.

## .work — Agent Scratch Space

The [`.work/`](.work/) directory at the repo root is scratch space for agent-driven workflows that follow a **download → restructure → rename → review** cycle. The user reviews outputs before they go anywhere permanent (e.g. shipped to an external destination).

**Conventions**:
- Each session creates its own subdirectory — never dump files directly in `.work/`. This keeps parallel sessions from stepping on each other.
- Suggested naming: `.work/{YYYY-MM-DD}-{short-task-slug}/` — e.g. `.work/2026-01-15-example-export/`.
- Do not scatter temporary files elsewhere on the user's PC. Everything agent-produced that the user needs to review lives under `.work/`.

**Typical flow**:
1. Browser downloads raw files to the active Playwright server's `output/` directory (e.g. `playwright/persistent/output/` for stateful portal work, or `playwright/headless/output/<session>/` for headless scrapes — Playwright auto-captures `download` events). Each server has its own output dir; see the Playwright usage policy above.
2. Agent moves + renames them into `.work/{session-dir}/` with clear, final names.
3. User reviews the structured session directory.
4. On approval, the files are shipped to their destination.

`.work/` is gitignored entirely and created on demand (nothing under it is committed).
