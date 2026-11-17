# Claude Code Project Guide

This file provides guidance for Claude Code when working on the HitsterCardGenerator project.

## Subfolder instructions

Before working in a subfolder, read any AGENTS.md from that folder up to the repo root that you haven't seen yet. Claude Code attaches them only when a file there is read: https://github.com/anthropics/claude-code/tree/main/mods/agents-md#where-it-still-differs-from-claudemd

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

## Browser usage policy

Browser automation goes through BrowserAI: the `browserai` MCP server in [.mcp.json](.mcp.json), which BrowserAI's installer also registers for every project. Its tools are `mcp__browserai__*`. Apply this policy:

1. **Open a session first.** `browserai_init` creates one in a new directory you name, for example `.work/{YYYY-MM-DD}-{short-task-slug}/browser/`; `browserai_resume` reopens one that exists. Pass that directory as `session` to every `browser_*` call. `browserai_init` takes a `purpose`, and every later call a `why`.
2. **Use the browser as little as possible.** After one research round on a new site, evaluate whether the task can be done by calling the underlying API directly (with credentials you already have). Prefer direct HTTP over browser automation: it is faster, more reliable, and easier to debug.
3. **To reverse-engineer a site's API shapes** (request/response, auth flow), open the session with `captureNetwork: true`, which writes an HTTP Archive into the session's `output` folder, or read `browser_network_requests`. The goal is usually to *stop* using the browser on that site: capture enough to make direct calls work, then switch. The archive holds every header and cookie in plain text, so delete it when done.
4. **Run without a window (`headed: false`)** unless a credentialed login is required and the credentials are not already on hand. Then tell the user what you are about to do and use `headed: true`. Do not see, ask for, or attempt to capture credentials: the user types them directly in the visible window.
5. **Destroy the session with `browserai_destroy` when the work is done**, promptly when it held a login: its logins and cookies stay in the profile on disk until then. Keep a logged-in session for later only when the user explicitly instructs you to. `browserai_destroy` deletes the whole session directory, screenshots and downloads included, so move out what must be kept first.

## .work — Agent Scratch Space

The [`.work/`](.work/) directory at the repo root is scratch space for agent-driven workflows that follow a **download → restructure → rename → review** cycle. The user reviews outputs before they go anywhere permanent (e.g. shipped to an external destination).

**Conventions**:
- Each session creates its own subdirectory — never dump files directly in `.work/`. This keeps parallel sessions from stepping on each other.
- Suggested naming: `.work/{YYYY-MM-DD}-{short-task-slug}/` — e.g. `.work/2026-01-15-example-export/`.
- Do not scatter temporary files elsewhere on the user's PC. Everything agent-produced that the user needs to review lives under `.work/`.

**Typical flow**:
1. Browser downloads raw files to the BrowserAI session's `output` folder, inside the session directory (Playwright auto-captures `download` events); see the Browser usage policy above.
2. Agent moves + renames them into `.work/{session-dir}/` with clear, final names.
3. User reviews the structured session directory.
4. On approval, the files are shipped to their destination.

`.work/` is gitignored entirely and created on demand (nothing under it is committed).
