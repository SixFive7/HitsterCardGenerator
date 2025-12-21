# Phase 8: Card Preview - Research

**Researched:** 2025-12-21
**Domain:** Svelte 5 card carousel with color customization
**Confidence:** HIGH

<research_summary>
## Summary

Researched the Svelte 5 ecosystem for building an interactive card preview with carousel navigation, genre color customization, and print-accurate rendering.

The standard approach uses Embla Carousel (via embla-carousel-svelte) for smooth touch-friendly navigation, svelte-awesome-color-picker v4 for color selection, and CSS aspect-ratio matching for preview accuracy.

Key finding: Don't try to render physical mm dimensions on screen - screen DPI varies too much. Instead, maintain the 85:55mm aspect ratio (17:11) at comfortable screen sizes. The PDF export will use actual mm dimensions.

**Primary recommendation:** Use Embla + svelte-awesome-color-picker + Svelte 5 $state runes for reactive customization. Keep card preview proportionally accurate, not physically accurate.
</research_summary>

<standard_stack>
## Standard Stack

### Core
| Library | Version | Purpose | Why Standard |
|---------|---------|---------|--------------|
| embla-carousel-svelte | 8.x | Carousel navigation | Best Svelte 5 support, touch-friendly, lightweight |
| svelte-awesome-color-picker | 4.x | Color picker | Rewritten for Svelte 5, highly customizable, accessible |

### Supporting
| Library | Version | Purpose | When to Use |
|---------|---------|---------|-------------|
| colord | (dep) | Color conversion | Bundled with color picker, use for hex/rgb/hsv |

### Alternatives Considered
| Instead of | Could Use | Tradeoff |
|------------|-----------|----------|
| Embla | Flowbite carousel | Flowbite heavier, Embla more flexible |
| Embla | svelte-light-carousel | Embla has better API, more plugins |
| svelte-awesome-color-picker | Native input[type=color] | Native is simpler but less customizable |

**Installation:**
```bash
npm install embla-carousel-svelte svelte-awesome-color-picker -D
```
</standard_stack>

<architecture_patterns>
## Architecture Patterns

### Recommended Component Structure
```
src/lib/
├── CardPreview/
│   ├── CardCarousel.svelte     # Embla carousel wrapper
│   ├── CardFront.svelte        # QR code side
│   ├── CardBack.svelte         # Info side (artist, year, title, genre)
│   └── CardControls.svelte     # Include/exclude, flip, navigation
├── ColorSettings/
│   ├── GenreColorPicker.svelte # Per-genre color picker
│   └── ColorPalettes.svelte    # Preset palette selection
└── stores/
    └── cardCustomization.ts    # Svelte 5 state for customizations
```

### Pattern 1: Embla Carousel with Card Navigation
**What:** Use Embla's action-based API for carousel
**When to use:** Any card-flipping / deck navigation UI
**Example:**
```svelte
<script>
  import emblaCarouselSvelte from 'embla-carousel-svelte'

  let emblaApi = $state(null)
  let selectedIndex = $state(0)

  function onInit(event) {
    emblaApi = event.detail
    emblaApi.on('select', () => {
      selectedIndex = emblaApi.selectedScrollSnap()
    })
  }
</script>

<div class="embla" use:emblaCarouselSvelte onemblaInit={onInit}>
  <div class="embla__container">
    {#each cards as card, i}
      <div class="embla__slide">
        <Card {card} />
      </div>
    {/each}
  </div>
</div>

<button onclick={() => emblaApi?.scrollPrev()}>Previous</button>
<span>{selectedIndex + 1} / {cards.length}</span>
<button onclick={() => emblaApi?.scrollNext()}>Next</button>
```

### Pattern 2: Color Picker with Genre Binding
**What:** Bind color picker to genre-specific colors
**When to use:** Per-genre color customization
**Example:**
```svelte
<script>
  import ColorPicker from 'svelte-awesome-color-picker'

  // From store/props
  let genreColors = $state({})
  let currentGenre = $props()

  let hex = $derived(genreColors[currentGenre] ?? '#808080')
</script>

<ColorPicker
  bind:hex
  position="responsive"
  on:input={() => genreColors[currentGenre] = hex}
/>
```

### Pattern 3: Card Size with Aspect Ratio
**What:** Use aspect-ratio CSS for proportionally accurate cards
**When to use:** Screen preview of print-sized cards
**Example:**
```svelte
<style>
  .card {
    /* 85mm x 55mm = 17:11 aspect ratio */
    aspect-ratio: 17 / 11;
    width: min(400px, 90vw);
    /* Card styling */
    border-radius: 8px;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  }
</style>

<div class="card">
  <!-- Card content -->
</div>
```

### Anti-Patterns to Avoid
- **Trying to match physical mm on screen:** Screen DPI varies (96-220+), mm units are unreliable for screen preview
- **Storing customization in component state:** Use shared state/store for persistence across views
- **Not cleaning up Embla on unmount:** Embla wrapper handles this, but manual API usage needs cleanup
</architecture_patterns>

<dont_hand_roll>
## Don't Hand-Roll

Problems that look simple but have existing solutions:

| Problem | Don't Build | Use Instead | Why |
|---------|-------------|-------------|-----|
| Carousel navigation | CSS scroll-snap carousel | Embla Carousel | Touch handling, momentum, API for programmatic control |
| Color picker UI | Custom sliders/inputs | svelte-awesome-color-picker | HSV/RGB conversion, accessibility, mobile support |
| Swipe detection | Touch event math | Embla | Edge cases (multi-touch, momentum, boundaries) |
| Color format conversion | Manual hex/rgb/hsv math | colord (via picker) | Edge cases, alpha, color spaces |

**Key insight:** UI interaction components (carousels, color pickers) have subtle complexity in touch handling, accessibility, and edge cases. Using battle-tested libraries prevents frustrating UX bugs.
</dont_hand_roll>

<common_pitfalls>
## Common Pitfalls

### Pitfall 1: Screen Size ≠ Print Size
**What goes wrong:** Trying to show exact 85x55mm cards on screen
**Why it happens:** CSS mm units are relative to 96 DPI assumption, actual screen DPI varies
**How to avoid:** Use aspect-ratio (17:11) at comfortable screen size, trust PDF for actual dimensions
**Warning signs:** Cards look too small or inconsistent across devices

### Pitfall 2: Carousel State After Navigation
**What goes wrong:** Current card index desyncs from Embla state
**Why it happens:** Not subscribing to Embla's 'select' event
**How to avoid:** Use emblaApi.on('select') to sync index with component state
**Warning signs:** Navigation buttons show wrong position, card counter incorrect

### Pitfall 3: Color State Reactivity
**What goes wrong:** Color changes don't update card preview immediately
**Why it happens:** One-way binding or missing reactive dependency
**How to avoid:** Use $state for colors, $derived for computed values
**Warning signs:** Need to navigate away and back to see color changes

### Pitfall 4: Card Flip Animation Complexity
**What goes wrong:** Flip animation feels janky or breaks layout
**Why it happens:** 3D transforms require preserve-3d, backface-visibility, proper z-index
**How to avoid:** Use simple CSS transform: rotateY(180deg) with transition
**Warning signs:** Seeing through card, flicker during animation
</common_pitfalls>

<code_examples>
## Code Examples

Verified patterns from official sources:

### Embla Carousel Basic Setup
```svelte
<!-- Source: embla-carousel.com/get-started/svelte/ -->
<script>
  import emblaCarouselSvelte from 'embla-carousel-svelte'

  let options = { loop: false, align: 'center' }
</script>

<div class="embla overflow-hidden" use:emblaCarouselSvelte={{ options }}>
  <div class="embla__container flex">
    {#each items as item}
      <div class="embla__slide flex-[0_0_100%] min-w-0">
        {item}
      </div>
    {/each}
  </div>
</div>
```

### Color Picker Basic Usage
```svelte
<!-- Source: svelte-awesome-color-picker.vercel.app -->
<script>
  import ColorPicker from 'svelte-awesome-color-picker'

  let hex = $state('#f6f0dc')
</script>

<ColorPicker bind:hex position="responsive" />
<p>Selected: {hex}</p>
```

### Card Flip with CSS
```svelte
<!-- Pattern: CSS 3D flip card -->
<script>
  let flipped = $state(false)
</script>

<div
  class="card-container perspective-1000"
  onclick={() => flipped = !flipped}
>
  <div class="card relative transition-transform duration-500 transform-style-preserve-3d"
       class:rotate-y-180={flipped}>
    <div class="card-front absolute inset-0 backface-hidden">
      <!-- Front content (QR code) -->
    </div>
    <div class="card-back absolute inset-0 backface-hidden rotate-y-180">
      <!-- Back content (info) -->
    </div>
  </div>
</div>

<style>
  .perspective-1000 { perspective: 1000px; }
  .transform-style-preserve-3d { transform-style: preserve-3d; }
  .backface-hidden { backface-visibility: hidden; }
  .rotate-y-180 { transform: rotateY(180deg); }
</style>
```
</code_examples>

<sota_updates>
## State of the Art (2024-2025)

| Old Approach | Current Approach | When Changed | Impact |
|--------------|------------------|--------------|--------|
| svelte-carousel | Embla Carousel | 2024 | Better Svelte 5 support, more maintained |
| svelte-color-picker | svelte-awesome-color-picker v4 | 2024 | Rewritten for Svelte 5 runes |
| Svelte stores | $state runes | Svelte 5 (2024) | Native reactivity, simpler API |

**New tools/patterns to consider:**
- **Svelte 5 snippets:** For reusable card template parts
- **CSS aspect-ratio:** Native browser support for maintaining proportions

**Deprecated/outdated:**
- **$: reactive statements:** Replaced by $derived in Svelte 5
- **on: event syntax:** Deprecated in Svelte 5, use onevent handlers
</sota_updates>

<open_questions>
## Open Questions

Things that couldn't be fully resolved:

1. **QR Code Display in Preview**
   - What we know: Backend generates QR as byte[] PNG, frontend needs to display it
   - What's unclear: Best way to pass QR data to frontend (base64 data URL vs served endpoint)
   - Recommendation: Use base64 data URL for simplicity, matches existing CardData pattern

2. **Color Palette Persistence**
   - What we know: User customizes genre colors, should persist across sessions
   - What's unclear: localStorage vs backend storage vs URL params
   - Recommendation: Start with localStorage for simplicity, consider backend if cross-device needed
</open_questions>

<sources>
## Sources

### Primary (HIGH confidence)
- [Embla Carousel Svelte](https://www.embla-carousel.com/get-started/svelte/) - installation, usage, Svelte 5 notes
- [svelte-awesome-color-picker](https://svelte-awesome-color-picker.vercel.app/) - installation, API, v4 Svelte 5 rewrite
- [CSS @page size](https://developer.mozilla.org/en-US/docs/Web/CSS/@page/size) - print sizing

### Secondary (MEDIUM confidence)
- [Printing correct size using CSS](https://stianjo.no/print-css-correct-size/) - mm units work for print, not screen
- [shadcn-svelte carousel](https://www.shadcn-svelte.com/docs/components/carousel) - Embla-based, confirms standard choice

### Tertiary (LOW confidence - needs validation)
- None - all findings verified against official sources
</sources>

<metadata>
## Metadata

**Research scope:**
- Core technology: Svelte 5 + Tailwind v4
- Ecosystem: Embla, svelte-awesome-color-picker
- Patterns: Carousel, color picker, CSS print preview
- Pitfalls: Screen vs print sizing, carousel state, color reactivity

**Confidence breakdown:**
- Standard stack: HIGH - well-maintained, Svelte 5 compatible
- Architecture: HIGH - follows established Svelte patterns
- Pitfalls: HIGH - documented in official sources
- Code examples: HIGH - from official documentation

**Research date:** 2025-12-21
**Valid until:** 2026-01-21 (30 days - stable ecosystem)
</metadata>

---

*Phase: 08-card-preview*
*Research completed: 2025-12-21*
*Ready for planning: yes*
