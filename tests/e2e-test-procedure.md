# E2E Test Procedure using Chrome DevTools MCP

This document describes how to run a complete E2E test of the Hitster Card Generator application using Chrome DevTools MCP tools.

## Prerequisites

1. Start the application: `dotnet run`
2. Ensure Spotify credentials are configured in environment variables
3. Have Chrome DevTools MCP tools available in Claude Code

## Test Steps

### Step 1: Navigate to Application

**Tool:** `mcp__chrome-devtools__navigate_page`
**Parameters:**
- URL: http://localhost:5000
- type: url

**Expected:** Page loads successfully

---

### Step 2: Verify Landing Page

**Tool:** `mcp__chrome-devtools__take_snapshot`

**Expected Elements:**
- "Hitster Card Generator" heading
- "Connected to backend" status message
- "Upload CSV" button
- "Build Playlist" button

---

### Step 3: Navigate to Playlist Builder

For this test, we use the Playlist Builder flow (simpler than CSV upload).

**Tool:** `mcp__chrome-devtools__click`
**Target:** "Build Playlist" button

**Tool:** `mcp__chrome-devtools__take_snapshot`
**Expected:** Search interface visible with "Search for songs on Spotify"

---

### Step 4: Search for a Song

**Tool:** `mcp__chrome-devtools__fill`
**Element:** Search input field
**Value:** "Billie Jean Michael Jackson"

**Tool:** `mcp__chrome-devtools__click`
**Target:** Search button

**Tool:** `mcp__chrome-devtools__wait_for`
**Text:** "Billie Jean"

**Expected:** Search results appear with matching tracks

---

### Step 5: Add Song to Playlist

**Tool:** `mcp__chrome-devtools__click`
**Target:** "Add" button for the first search result

**Tool:** `mcp__chrome-devtools__take_snapshot`
**Expected:** Playlist section shows the added song, count shows "1 song"

---

### Step 6: Continue to Preview

**Tool:** `mcp__chrome-devtools__click`
**Target:** "Continue to Preview" button

**Tool:** `mcp__chrome-devtools__wait_for`
**Text:** "Preview Your Cards"

**Tool:** `mcp__chrome-devtools__take_snapshot`
**Expected:** Card preview page with carousel visible

---

### Step 7: Test Card Flip

**Tool:** `mcp__chrome-devtools__click`
**Target:** "Flip" button or card element

**Tool:** `mcp__chrome-devtools__take_screenshot`
**Expected:** Card shows back side with song info and album art

---

### Step 8: Navigate to Export

**Tool:** `mcp__chrome-devtools__click`
**Target:** "Continue to Export" button

**Tool:** `mcp__chrome-devtools__take_snapshot`
**Expected:** "Export Your Cards" heading, export button visible

---

### Step 9: Download PDF

**Tool:** `mcp__chrome-devtools__click`
**Target:** "Download PDF" button

**Tool:** `mcp__chrome-devtools__wait_for`
**Text:** "Download Started!" or similar success message

**Tool:** `mcp__chrome-devtools__take_screenshot`
**Expected:** Success message or download initiated

---

### Step 10: Test Start New Batch

**Tool:** `mcp__chrome-devtools__click`
**Target:** "Start New Batch" button

**Tool:** `mcp__chrome-devtools__take_snapshot`
**Expected:** Returns to landing page with "Upload CSV" and "Build Playlist" options

---

## Success Criteria

- [ ] All steps complete without errors
- [ ] Landing page loads with backend connected
- [ ] Playlist builder search returns Spotify results
- [ ] Songs can be added to playlist
- [ ] Card preview displays correctly
- [ ] Card flip functionality works
- [ ] PDF export initiates successfully
- [ ] Flow can be restarted via "Start New Batch"

## Notes

- This test uses the Playlist Builder flow as it requires less setup than CSV upload
- The same card preview and export functionality is tested regardless of input method
- Screenshots should be captured at key steps for visual verification
