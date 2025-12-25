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
- Devcontainer available for sandbox-free Claude Code operation

## Documentation

When making any changes to the project ensure the README.md is updated to reflect those changes.

## Visual Verification

Use Chrome DevTools MCP tools for visual verification instead of human checkpoints:
- `mcp__chrome-devtools__take_screenshot` - Capture page/element screenshots
- `mcp__chrome-devtools__take_snapshot` - Get accessibility tree snapshot
- `mcp__chrome-devtools__navigate_page` - Navigate to URLs for testing

Use these to verify UI changes yourself rather than asking the user to check.
