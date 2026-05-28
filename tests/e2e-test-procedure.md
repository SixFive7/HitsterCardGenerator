# E2E Test Procedure (Playwright MCP)

This document describes how to run a complete E2E test of the Hitster Card Generator application using the project's `playwright-headless` MCP server (see [../playwright/](../playwright/)).

## Prerequisites

1. Start the application: `dotnet run`
2. Ensure Spotify credentials are configured in environment variables
3. Have the `playwright-headless` MCP server available in Claude Code (configured in [../.mcp.json](../.mcp.json))

All tools below use the `mcp__playwright-headless__browser_*` prefix. The same procedure can be run against `playwright-tracing` if a network trace is wanted.

## Test Steps

### Step 1: Navigate to Application

**Tool:** `mcp__playwright-headless__browser_navigate`
**Parameters:**
- URL: http://localhost:5657

**Expected:** Landing page loads with hero section and "Get Started" buttons.

### Step 2: Verify Landing Page

**Tool:** `mcp__playwright-headless__browser_snapshot`

**Expected Elements:**
- Heading: "Hitster Card Generator"
- "Upload CSV" button
- "Build Playlist" button
- Spotify connection indicator showing connected

### Step 3: Navigate to Playlist Builder

For this test, we use the Playlist Builder flow (simpler than CSV upload).

**Tool:** `mcp__playwright-headless__browser_click`
**Target:** "Build Playlist" button

**Tool:** `mcp__playwright-headless__browser_snapshot`
**Expected:** Search interface visible with "Search for songs on Spotify"

### Step 4: Search for a Song

**Tool:** `mcp__playwright-headless__browser_type`
**Element:** Search input field
**Value:** "Billie Jean Michael Jackson"

**Tool:** `mcp__playwright-headless__browser_click`
**Target:** Search button

**Tool:** `mcp__playwright-headless__browser_wait_for`
**Text:** "Billie Jean"

**Expected:** Search results appear with Michael Jackson's "Billie Jean"

### Step 5: Add Song to Playlist

**Tool:** `mcp__playwright-headless__browser_click`
**Target:** "Add" button for the first search result

**Tool:** `mcp__playwright-headless__browser_snapshot`
**Expected:** Playlist section shows the added song, count shows "1 song"

### Step 6: Continue to Preview

**Tool:** `mcp__playwright-headless__browser_click`
**Target:** "Continue to Preview" button

**Tool:** `mcp__playwright-headless__browser_wait_for`
**Text:** "Preview Your Cards"

**Tool:** `mcp__playwright-headless__browser_snapshot`
**Expected:** Card preview page with carousel visible

### Step 7: Test Card Flip

**Tool:** `mcp__playwright-headless__browser_click`
**Target:** "Flip" button or card element

**Tool:** `mcp__playwright-headless__browser_take_screenshot`
**Expected:** Card shows back side with song info and album art

### Step 8: Navigate to Export

**Tool:** `mcp__playwright-headless__browser_click`
**Target:** "Continue to Export" button

**Tool:** `mcp__playwright-headless__browser_snapshot`
**Expected:** "Export Your Cards" heading, export button visible

### Step 9: Download PDF

**Tool:** `mcp__playwright-headless__browser_click`
**Target:** "Download PDF" button

**Tool:** `mcp__playwright-headless__browser_wait_for`
**Text:** "Download Started!" or similar success message

**Tool:** `mcp__playwright-headless__browser_take_screenshot`
**Expected:** Success message or download initiated. The PDF file is captured by Playwright's `download` event handler and lands in the session's output directory (see [../playwright/README.md](../playwright/README.md) — "Server-generated downloads" rule).

### Step 10: Test Start New Batch

**Tool:** `mcp__playwright-headless__browser_click`
**Target:** "Start New Batch" button

**Tool:** `mcp__playwright-headless__browser_snapshot`
**Expected:** Returns to landing page with "Upload CSV" and "Build Playlist" options
