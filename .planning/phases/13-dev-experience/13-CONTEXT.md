# Phase 13: Dev Experience - Context

**Gathered:** 2025-12-23
**Status:** Ready for planning

<vision>
## How This Should Work

One-click F5 launch that starts everything. Press F5 in VS Code, the .NET backend starts, Vite dev server starts, and the browser opens automatically to localhost. HMR works so frontend changes reflect instantly without full reload.

No separate terminal windows to manage, no remembering which order to start things in. Just F5 and you're developing.

</vision>

<essential>
## What Must Be Nailed

- **HMR actually working** - Frontend changes reflect instantly without full reload
- **Single F5 launch** - Not having to start backend and frontend separately
- **Fast debug cycles** - No unnecessary npm build steps slowing down iteration

All three are equally important for this phase.

</essential>

<boundaries>
## What's Out of Scope

- Docker integration - that's Phase 14
- Production build optimization - focus is purely on dev experience
- This phase is VS Code dev workflow only

</boundaries>

<specifics>
## Specific Ideas

Standard VS Code compound launch configs work fine. No special proxy requirements - using the existing setup where .NET serves static files and Vite runs separately for HMR.

</specifics>

<notes>
## Additional Context

The existing work from Phase 12 (devcontainer) provides the dev environment. This phase builds on that by making the inner development loop fast and seamless.

Current pain point: Need to fix NpmBuild to only run in Release mode so debug builds are fast.

</notes>

---

*Phase: 13-dev-experience*
*Context gathered: 2025-12-23*
