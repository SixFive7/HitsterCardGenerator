# Phase 7: Spotify Web Flow - Research

**Researched:** 2025-12-21
**Domain:** Spotify track matching UI with confidence display
**Confidence:** HIGH (existing service + standard UI patterns)

<research_summary>
## Summary

Researched UI patterns for displaying Spotify track matches with confidence indicators and alternative selection. The core Spotify integration already exists in SpotifyService.cs with Client Credentials authentication and smart match selection.

Key finding: Match confidence should use categorical indicators (High/Medium/Low) with color coding, not precise percentages. Alternative selection should use click-to-select inline patterns rather than modals.

**Primary recommendation:** Extend existing SpotifyService with album art URLs, add confidence scoring based on string matching, display results with color-coded confidence badges and inline alternatives.
</research_summary>

<standard_stack>
## Standard Stack

### Already Implemented (v1.0)
| Library | Version | Purpose | Status |
|---------|---------|---------|--------|
| SpotifyAPI.Web | latest | Spotify API client | ✓ Working |
| SpotifyService | - | Auth + search + smart selection | ✓ Working |
| SpotifySearchResult | - | Track result model | ✓ Working |

### Needed for Phase 7
| Library | Version | Purpose | Why |
|---------|---------|---------|-----|
| Flowbite Svelte | latest | Progress bar component | Pre-built, animated, Tailwind-compatible |
| Existing Tailwind | v4 | Styling | Already configured in Phase 5 |

### No New Libraries Required
The existing stack handles this phase. SpotifyAPI.Web already returns album art URLs in the `FullTrack.Album.Images` collection - we just need to expose it in our model.
</standard_stack>

<architecture_patterns>
## Architecture Patterns

### Recommended Flow
```
CSV Upload → Validation Display → Auto-Match Trigger
                                      ↓
                              [Progress Indicator]
                                      ↓
                              Results Display
                                      ↓
                              [Click for Alternatives]
```

### Pattern 1: Streaming Progress Updates
**What:** Use Server-Sent Events (SSE) or polling for real-time progress
**When to use:** Bulk operations where users need feedback
**Recommendation:** Simple polling (fetch every 500ms) is sufficient for 10-50 songs. SSE is overkill for this scale.

```typescript
// Svelte component pattern
let progress = $state({ completed: 0, total: 0, current: '' });

async function startMatching(songs: Song[]) {
  const response = await fetch('/api/match', {
    method: 'POST',
    body: JSON.stringify({ songs })
  });
  const taskId = await response.json();

  // Poll for progress
  const interval = setInterval(async () => {
    const status = await fetch(`/api/match/${taskId}/status`).then(r => r.json());
    progress = status;
    if (status.completed === status.total) clearInterval(interval);
  }, 500);
}
```

### Pattern 2: Confidence Badge Display
**What:** Color-coded categorical badges (High/Medium/Low)
**When to use:** Displaying AI/algorithmic match quality
**Example:**

```svelte
<script>
  const confidenceColors = {
    high: 'bg-green-500',
    medium: 'bg-amber-500',
    low: 'bg-red-500'
  };
</script>

<span class="px-2 py-1 rounded-full text-white text-xs {confidenceColors[confidence]}">
  {confidence === 'high' ? 'Exact' : confidence === 'medium' ? 'Close' : 'Uncertain'}
</span>
```

### Pattern 3: Inline Alternative Selection
**What:** Click anywhere on alternative row to select it (no buttons/modals)
**When to use:** Quick corrections in a list
**Example:**

```svelte
<div class="alternatives">
  {#each alternatives as alt}
    <button
      class="w-full text-left p-2 hover:bg-gray-100 cursor-pointer"
      onclick={() => selectAlternative(alt)}
    >
      <img src={alt.albumArt} class="w-10 h-10 rounded" />
      <span>{alt.trackName}</span> - <span class="text-gray-500">{alt.artistName}</span>
    </button>
  {/each}
</div>
```

### Anti-Patterns to Avoid
- **Modal dialogs for alternatives:** Interrupts flow, adds clicks
- **Precise percentage confidence:** "87.3% match" is false precision
- **Blocking on unmatched songs:** Show "Not found" but allow proceeding
</architecture_patterns>

<dont_hand_roll>
## Don't Hand-Roll

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Spotify authentication | Custom OAuth | Existing SpotifyService | Already working, tested |
| Track selection logic | New algorithm | Existing SelectBestMatch | Already handles album > single > compilation, remaster detection |
| Progress bar animation | CSS from scratch | Flowbite/Tailwind classes | Smooth transitions built-in |
| String similarity | Levenshtein from scratch | Simple contains/equals | Spotify already fuzzy-matches; we just need to verify return matches input |

**Key insight:** The SpotifyService already does the hard work. Phase 7 is primarily UI: displaying results, showing confidence, and handling corrections.
</dont_hand_roll>

<common_pitfalls>
## Common Pitfalls

### Pitfall 1: Over-Engineering Confidence Scores
**What goes wrong:** Complex fuzzy matching algorithms that don't improve results
**Why it happens:** Assuming Spotify's search is bad (it's not)
**How to avoid:** Use simple heuristics:
  - Exact match (case-insensitive) = High
  - Track returned but artist/title differs slightly = Medium
  - No results or very different = Low
**Warning signs:** Spending time on Levenshtein distance calculations

### Pitfall 2: Blocking UI During Matching
**What goes wrong:** User stares at frozen screen during bulk match
**Why it happens:** Not implementing progress feedback
**How to avoid:** Show progress indicator with current song being processed
**Warning signs:** No visual feedback after clicking "Match"

### Pitfall 3: Missing Album Art
**What goes wrong:** Results list looks boring, hard to scan
**Why it happens:** Forgetting to fetch/display album artwork
**How to avoid:** Extend SpotifySearchResult to include `AlbumImageUrl`
**Warning signs:** Text-only results display

### Pitfall 4: Not Showing Alternatives Initially
**What goes wrong:** User has to click to see there are alternatives
**Why it happens:** Hiding alternatives behind "Show more" button
**How to avoid:** If confidence < High, show top 3 alternatives inline or with single click expansion
**Warning signs:** Extra clicks required for common correction flow
</common_pitfalls>

<code_examples>
## Code Examples

### Extending SpotifySearchResult for Album Art
```csharp
// Add to SpotifySearchResult.cs
public string AlbumImageUrl { get; init; } = string.Empty;

// In SpotifyService.MapToSearchResult()
AlbumImageUrl = track.Album?.Images?.FirstOrDefault()?.Url ?? string.Empty
```

### Confidence Calculation (Simple Heuristic)
```csharp
public string CalculateConfidence(SpotifySearchResult result, string searchArtist, string searchTitle)
{
    var titleMatch = result.TrackName.Equals(searchTitle, StringComparison.OrdinalIgnoreCase);
    var artistMatch = result.ArtistName.Equals(searchArtist, StringComparison.OrdinalIgnoreCase);

    if (titleMatch && artistMatch) return "high";
    if (titleMatch || artistMatch) return "medium";
    return "low";
}
```

### Match Result DTO for API
```csharp
public record MatchResult
{
    public int Index { get; init; }
    public string OriginalTitle { get; init; }
    public string OriginalArtist { get; init; }
    public SpotifySearchResult? Match { get; init; }
    public string Confidence { get; init; } // "high", "medium", "low", "none"
    public List<SpotifySearchResult> Alternatives { get; init; } = new();
}
```

### Svelte Match Result Component
```svelte
<script>
  let { result, onSelectAlternative } = $props();
  let showAlternatives = $state(false);
</script>

<div class="flex items-center gap-4 p-4 border-b">
  {#if result.match}
    <img src={result.match.albumImageUrl} alt="" class="w-16 h-16 rounded" />
    <div class="flex-1">
      <div class="font-medium">{result.match.trackName}</div>
      <div class="text-sm text-gray-500">{result.match.artistName}</div>
    </div>
    <ConfidenceBadge confidence={result.confidence} />
    <button onclick={() => showAlternatives = !showAlternatives}>
      Change
    </button>
  {:else}
    <div class="w-16 h-16 bg-gray-200 rounded flex items-center justify-center">
      <span class="text-gray-400">?</span>
    </div>
    <div class="flex-1 text-gray-500">Not found: {result.originalTitle}</div>
  {/if}
</div>

{#if showAlternatives && result.alternatives.length > 0}
  <div class="ml-20 bg-gray-50 rounded">
    {#each result.alternatives as alt}
      <button class="w-full text-left p-3 hover:bg-gray-100" onclick={() => onSelectAlternative(result.index, alt)}>
        <img src={alt.albumImageUrl} class="w-10 h-10 rounded inline mr-2" />
        {alt.trackName} - {alt.artistName}
      </button>
    {/each}
  </div>
{/if}
```
</code_examples>

<sota_updates>
## State of the Art (2024-2025)

| Old Approach | Current Approach | Impact |
|--------------|------------------|--------|
| Precise % confidence | Categorical (High/Medium/Low) | Less false precision, clearer UX |
| Modal dialogs for selection | Inline click-to-select | Faster corrections |
| WebSockets for progress | SSE or polling | Simpler, works well at this scale |

**Svelte 5 runes:**
- Use `$state` for reactive progress values
- Use `$effect` for progress polling cleanup
- Already configured in Phase 5

**No major ecosystem changes** affecting this phase.
</sota_updates>

<open_questions>
## Open Questions

1. **Progress API design**
   - What we know: Need endpoint for progress status
   - Options: Polling vs SSE vs returning all at once
   - Recommendation: Start with synchronous batch match (return all results at once), add polling only if >50 songs causes timeout

2. **Alternatives count**
   - What we know: Context says "3-5 alternatives"
   - Current: SpotifyService returns up to 10 results
   - Recommendation: Show top 3, expand to 5 on demand
</open_questions>

<sources>
## Sources

### Primary (HIGH confidence)
- Existing SpotifyService.cs - reviewed implementation
- Existing SpotifySearchResult.cs - reviewed model
- SpotifyAPI.Web library - FullTrack.Album.Images for album art

### Secondary (MEDIUM confidence)
- [Confidence Visualization Patterns](https://agentic-design.ai/patterns/ui-ux-patterns/confidence-visualization-patterns) - color coding, threshold recommendations
- [Baymard Autocomplete Design](https://baymard.com/blog/autocomplete-design) - click-to-select patterns
- [Flowbite Svelte Progress](https://flowbite-svelte.com/docs/components/progress) - progress component
- [shadcn-svelte Progress](https://www.shadcn-svelte.com/docs/components/progress) - alternative progress component

### Tertiary (LOW confidence - needs validation)
- None - all patterns verified against established sources
</sources>

<metadata>
## Metadata

**Research scope:**
- Core technology: Existing SpotifyService + Svelte 5 UI
- Ecosystem: No new libraries needed
- Patterns: Confidence display, inline selection, progress indication
- Pitfalls: Over-engineering confidence, blocking UI, missing album art

**Confidence breakdown:**
- Standard stack: HIGH - using existing implementation
- Architecture: HIGH - standard web patterns
- Pitfalls: HIGH - common UX mistakes documented
- Code examples: HIGH - based on existing code + standard Svelte

**Research date:** 2025-12-21
**Valid until:** 2026-01-21 (30 days - stable patterns)
</metadata>

---

*Phase: 07-spotify-web-flow*
*Research completed: 2025-12-21*
*Ready for planning: yes*
