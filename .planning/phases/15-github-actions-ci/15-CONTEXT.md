# Phase 15: GitHub Actions CI - Context

**Gathered:** 2025-12-24
**Status:** Ready for planning

<vision>
## How This Should Work

When I push a semantic version tag like `v2.3.0`, a GitHub Actions workflow automatically kicks in. It runs the full verification gauntlet — build, tests, formatting checks — then builds the Docker image and pushes it to GHCR with both the version tag and `:latest`.

After a successful push, the workflow creates a GitHub Release. The release notes come from CHANGELOG.md, which Claude writes before I tag the release (as part of the GSD milestone completion flow). The changelog uses a hybrid format: a narrative intro that tells the story of what changed, followed by categorized lists (Added, Changed, Fixed, etc.).

The whole thing should be reliable and reasonably fast. Use caching for NuGet packages, npm modules, and Docker layers so we're not rebuilding the world every time. But never skip verification steps for speed — correctness first.

</vision>

<essential>
## What Must Be Nailed

- **Just works on tag push** — Push a semver tag, workflow runs, image appears in GHCR. No manual steps.
- **Full verification before publish** — Build, tests, and format checks must pass. Don't push broken images.
- **Proper image tagging** — Version tag (v2.3.0) plus `:latest` so users can pin or float.
- **Auto GitHub Release** — Release created with notes extracted from CHANGELOG.md. No manual release drafting.

</essential>

<boundaries>
## What's Out of Scope

- Multi-architecture builds (ARM64/AMD64) — that's Phase 16
- PR checks or branch builds — only release tags trigger this workflow
- Complex failure recovery — if it fails, fix and re-tag
- Notifications beyond GitHub UI — no Slack/Discord integration

</boundaries>

<specifics>
## Specific Ideas

- Use standard `docker/build-push-action` — the battle-tested pattern
- Tag pattern: `v*.*.*` (semantic versioning with v-prefix)
- CHANGELOG.md in hybrid format: narrative intro + categorized changes
- Claude rule integrates with GSD workflow to write release notes before tagging
- Smart caching for dependencies and Docker layers

</specifics>

<notes>
## Additional Context

The release notes workflow is: Claude analyzes commits since last release, writes a hybrid-format section in CHANGELOG.md, then I push the version tag. CI extracts the latest version's notes for the GitHub Release body.

This keeps release notes meaningful (human-readable stories) while automation handles the boring parts (extracting, publishing).

</notes>

---

*Phase: 15-github-actions-ci*
*Context gathered: 2025-12-24*
