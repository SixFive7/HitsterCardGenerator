# Phase 30: Automated E2E Testing - Context

**Gathered:** 2025-12-26
**Status:** Ready for planning

<vision>
## How This Should Work

Automated tests that run through the complete application flow without human intervention. When I want to verify the app works correctly, I should be able to run a command that:

1. Opens the application in Chrome
2. Uploads a CSV file with test songs
3. Goes through the Spotify matching process
4. Previews the cards and tests the flip functionality
5. Exports a PDF and verifies it was generated correctly

The tests use Chrome DevTools MCP for browser automation - the same tools Claude uses for visual verification during development. This means the tests can interact with the real UI, click buttons, upload files, and verify what's on screen.

Think of it as a "smoke test" that proves the whole flow works end-to-end. If these tests pass, we know the core user journey is intact.

</vision>

<essential>
## What Must Be Nailed

- **Complete flow coverage** - Tests must exercise the entire path: CSV upload -> Spotify match -> card preview -> flip cards -> PDF export
- **Card flip verification** - Specifically test that the flip functionality works (this was a bug that got fixed)
- **PDF export verification** - Confirm a PDF is actually generated and can be downloaded
- **Runnable tests** - Should be able to run these tests to verify the app works correctly after changes

</essential>

<boundaries>
## What's Out of Scope

- Unit tests or integration tests - this is specifically E2E browser testing
- Testing every edge case - focus on the happy path / main flow
- Cross-browser testing - Chrome only via DevTools MCP
- Performance testing - just verify functionality works
- Automated CI/CD integration - tests run manually for now

</boundaries>

<specifics>
## Specific Ideas

- Use Chrome DevTools MCP tools: `take_snapshot`, `take_screenshot`, `click`, `fill`, `navigate_page`, etc.
- Test data: Use a small test CSV with known songs that will match on Spotify
- Verification approach: Screenshots at key points to confirm UI state
- PDF verification: Confirm the download happens and file exists

</specifics>

<notes>
## Additional Context

This is the final phase of milestone v2.8 (Simplification). Completing this phase marks the milestone as done.

The app now has unified rendering where preview and PDF use the same server-generated card images. This makes verification simpler - if the preview looks right, the PDF should match.

The Chrome DevTools MCP is already available in the project environment and used for visual verification during development (per CLAUDE.md).

</notes>

---

*Phase: 30-automated-e2e-testing*
*Context gathered: 2025-12-26*
