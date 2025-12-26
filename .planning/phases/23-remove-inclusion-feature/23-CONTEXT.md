# Phase 23: Remove Inclusion Feature - Context

**Gathered:** 2025-12-26
**Status:** Ready for planning

<vision>
## How This Should Work

Clean sweep removal of the "included" button feature. When this phase is done, the feature should be completely gone — no UI button, no state management, no localStorage traces, no backend code. It should be as if the inclusion feature never existed.

The app continues to work exactly as before for all other functionality, just without this obsolete toggle.

</vision>

<essential>
## What Must Be Nailed

- **Clean code** — No dead code left behind, no orphaned functions, types, or state management related to inclusion
- Remove all traces: UI components, Svelte stores, localStorage keys, any backend references

</essential>

<boundaries>
## What's Out of Scope

- No specific exclusions — whatever it takes to completely remove the feature
- Don't need to preserve any inclusion-related code "just in case"

</boundaries>

<specifics>
## Specific Ideas

No specific requirements — open to standard removal approach. Just make it clean.

</specifics>

<notes>
## Additional Context

This is a cleanup phase — the inclusion feature is obsolete and adds unnecessary complexity. Priority is code cleanliness over preserving any backward compatibility with the removed feature.

</notes>

---

*Phase: 23-remove-inclusion-feature*
*Context gathered: 2025-12-26*
