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

## Visual verification & E2E testing

Four Playwright MCP servers are configured: `playwright-headless`, `playwright-interactive`, `playwright-tracing`, `playwright-persistent`. Apply this policy:

1. **Default to `playwright-headless`** for visual verification of UI changes and for running the E2E procedure ([tests/e2e-test-procedure.md](tests/e2e-test-procedure.md)). Parallel-safe, ephemeral, no state on disk.
2. **Use `playwright-tracing`** when debugging API/UI interactions and you need a network trace (HAR with WebSocket frames) or a full session log.
3. **`playwright-interactive` and `playwright-persistent`** are configured but rarely needed — the app has no built-in authentication. Use only when something genuinely requires a credentialed login or persistent profile state.

Use these to verify UI changes yourself rather than asking the user to check. See [playwright/README.md](playwright/README.md) for the architecture, per-mode settings, and rules (HAR archive convention, screenshot filename behaviour, etc.).
