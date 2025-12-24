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

When making significant changes to the project (new features, configuration changes, deployment updates), ensure the README.md is updated to reflect those changes.
