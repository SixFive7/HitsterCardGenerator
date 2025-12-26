# Phase 26: Unified Rendering - Research Document

## Executive Summary

This research evaluates approaches for unified card rendering to ensure pixel-perfect consistency between browser preview and PDF export. After analyzing five distinct approaches against our constraints (performance, simplicity, pixel-perfect accuracy), **Option A: QuestPDF Server-Side Image Generation** emerges as the recommended approach.

---

## Current Architecture Analysis

### Browser Preview (Svelte/CSS)
- **Files**: `CardFront.svelte`, `CardBack.svelte`, `CardCarousel.svelte`
- **Technology**: HTML + CSS with Svelte 5 reactivity
- **Layout**: Flexbox, aspect-ratio CSS (17:11), variable padding
- **Fonts**: System fonts with px sizing (56px year, 14px artist, 18px title, 12px genre)
- **Colors**: Dynamic text color calculation based on background brightness
- **Features**: 3D CSS flip animation via `transform: rotateY(180deg)`

### PDF Export (QuestPDF/.NET)
- **Files**: `CardDesigner.cs`, `PdfExporter.cs`
- **Technology**: QuestPDF library (v2025.12.0)
- **Layout**: Millimeter measurements (85x55mm card, 45mm QR)
- **Fonts**: QuestPDF fonts with pt sizes (24pt year, 9pt artist, 9pt title, 7pt genre)
- **Colors**: Hardcoded QuestPDF Colors (Grey.Darken2, Grey.Darken3, Grey.Medium)

### Key Differences Causing Mismatch
| Aspect | Browser Preview | PDF Export |
|--------|-----------------|------------|
| Year font size | 56px | 24pt (~32px) |
| Artist font size | 14px | 9pt (~12px) |
| Title font size | 18px | 9pt (~12px) |
| Genre font size | 12px | 7pt (~9px) |
| Text colors | Dynamic (brightness-based) | Hardcoded grey values |
| Front card | Album art (placeholder) | QR code |
| Spacing model | px/rem with flexbox | mm with Column layout |

---

## Research Findings by Option

### Option A: QuestPDF Server-Side Image Generation

**Approach**: Use QuestPDF's `GenerateImages()` to create PNG previews, serve via API endpoint

#### Technical Details
- QuestPDF already has `GenerateFrontCardImage()` and `GenerateBackCardImage()` methods in `CardDesigner.cs`
- Returns PNG byte arrays at configurable DPI (default 288 DPI)
- Single source of truth for card design

#### Performance Characteristics
- **Single card generation**: ~1-5ms per page (based on QuestPDF benchmarks)
- **Memory**: ~30MB spike per `GenerateImages()` call (known issue, GC handles cleanup)
- **Image size**: ~15-50KB per card at 288 DPI PNG
- **Network latency**: Add ~50-100ms for HTTP round-trip

#### Implementation Approach
```
Browser                        Server
   |                              |
   |-- GET /api/card/{id}/front --|
   |                              |-- QuestPDF.GenerateImages()
   |<-- PNG bytes (cached) -------|
   |                              |
   |-- GET /api/card/{id}/back ---|
   |                              |-- QuestPDF.GenerateImages()
   |<-- PNG bytes (cached) -------|
```

#### Caching Strategy
- **In-memory cache** keyed by: `{cardId}_{side}_{backgroundColor}`
- Cache invalidation on color change
- Pre-generate visible cards + N cards ahead

#### Impact on Card Flipping
- Pre-fetch both front/back on carousel navigation
- CSS flip animation on cached images (instant)
- Worst case: 50-150ms delay on first flip if not pre-fetched

#### Pros
- **Pixel-perfect**: Guaranteed match - same code generates preview and PDF
- **Simple architecture**: No new dependencies, leverages existing QuestPDF
- **Maintainable**: Single source of truth for card design
- **Already partially implemented**: `CardDesigner.cs` has image generation methods

#### Cons
- **Network dependency**: Requires server round-trip for previews
- **Memory spikes**: QuestPDF image generation has known memory behavior
- **Base64 overhead**: If embedding in HTML, 33% size increase
- **Flip latency**: Cold flip could have ~100ms delay

#### Scoring (1-5, higher is better)
| Criterion | Score | Notes |
|-----------|-------|-------|
| Performance | 3 | Good with caching, network adds latency |
| Simplicity | 5 | Minimal new code, uses existing infrastructure |
| Pixel-Perfect | 5 | Guaranteed - same rendering engine |
| **Total** | **13** | |

---

### Option B: Server-Side SVG Generation

**Approach**: Generate SVG strings on server, embed in browser for preview

#### Technical Details
- QuestPDF supports SVG as input but NOT as output format
- Would require custom SVG generation code in C#
- SVG renders natively in browser `<img>` tags

#### Implementation Complexity
- Must replicate QuestPDF layout logic in SVG format
- Two rendering codebases (SVG generator + QuestPDF for PDF)
- Font rendering differences between browser and PDF

#### Example SVG Card Back
```xml
<svg viewBox="0 0 85 55" xmlns="http://www.w3.org/2000/svg">
  <rect fill="#ff6b6b" width="85" height="55"/>
  <text x="42.5" y="10" text-anchor="middle" font-size="3mm">Artist</text>
  <text x="42.5" y="30" text-anchor="middle" font-size="8mm" font-weight="bold">1985</text>
  <text x="42.5" y="40" text-anchor="middle" font-size="3mm">Title</text>
  <text x="42.5" y="52" text-anchor="middle" font-size="2mm">GENRE</text>
</svg>
```

#### Pros
- **Vector graphics**: Infinitely scalable, small file size (~2-5KB)
- **Fast render**: No image decoding, native browser rendering
- **CSS animations**: Can animate SVG elements directly

#### Cons
- **Two rendering engines**: Must maintain parity between SVG and QuestPDF
- **Font differences**: Browser fonts vs QuestPDF fonts will differ
- **NOT pixel-perfect**: Cannot guarantee match without extensive testing
- **Complex**: More code to write and maintain

#### Scoring
| Criterion | Score | Notes |
|-----------|-------|-------|
| Performance | 5 | Very fast, small payload |
| Simplicity | 2 | Requires new SVG generation logic |
| Pixel-Perfect | 2 | Different rendering engines |
| **Total** | **9** | |

---

### Option C: Shared Rendering Library (SkiaSharp WASM)

**Approach**: Use SkiaSharp in both .NET backend and browser via WebAssembly

#### Technical Details
- SkiaSharp supports WebAssembly via `SkiaSharp.NativeAssets.WebAssembly`
- QuestPDF uses Skia internally (removed SkiaSharp dependency in 2024)
- Could define cards in SkiaSharp, use in both contexts

#### Architecture
```
Shared Card Definition (C#/SkiaSharp)
         |
    +---------+---------+
    |                   |
  Server              Browser (WASM)
  (QuestPDF)          (SkiaSharp.WASM)
```

#### Challenges
- Would need to abandon QuestPDF's fluent API
- WASM bundle size: ~3-5MB additional
- WASM cold start: 500-2000ms initial load
- Different SkiaSharp versions/builds for server vs WASM

#### Pros
- **True shared code**: Same C# code runs in both environments
- **Pixel-perfect potential**: Same underlying Skia engine
- **No network for preview**: Renders locally in browser

#### Cons
- **WASM overhead**: Large bundle, cold start penalty
- **Architecture change**: Significant refactoring from QuestPDF
- **Complexity**: Must manage two Skia builds
- **Maintenance burden**: WASM debugging is harder

#### Scoring
| Criterion | Score | Notes |
|-----------|-------|-------|
| Performance | 2 | WASM cold start, large bundle |
| Simplicity | 1 | Major architectural change |
| Pixel-Perfect | 4 | Same engine, but different builds |
| **Total** | **7** | |

---

### Option D: HTML-to-PDF Library (IronPDF/Puppeteer)

**Approach**: Define cards in HTML/CSS, convert to PDF using browser engine

#### Technical Details
- IronPDF: Commercial library, Chromium-based, ~$749/year
- PuppeteerSharp: Open source, spawns headless Chrome
- Keep Svelte components as-is, render to PDF server-side

#### Performance Considerations
- Puppeteer Docker image: 800MB-1.2GB
- Cold start: 2-10 seconds for Chrome spawn
- Per-page render: 100-500ms

#### Pros
- **Existing preview code**: No changes to Svelte components
- **Pixel-perfect**: Same browser renders preview and PDF
- **Modern CSS support**: Flexbox, Grid, CSS variables

#### Cons
- **Heavy dependency**: Full browser engine on server
- **Cold start**: Seconds to spawn browser
- **Cost**: IronPDF is commercial; Puppeteer has ops overhead
- **Security**: Running browser on server adds attack surface

#### Scoring
| Criterion | Score | Notes |
|-----------|-------|-------|
| Performance | 1 | Browser spawn overhead |
| Simplicity | 2 | Heavy dependency, ops complexity |
| Pixel-Perfect | 5 | Guaranteed - same browser engine |
| **Total** | **8** | |

---

### Option E: Hybrid CSS-Sync Approach

**Approach**: Keep Svelte preview, manually sync CSS values with QuestPDF

#### Technical Details
- Define shared constants (mm measurements, font ratios)
- Update Svelte CSS to match QuestPDF proportions
- Use CSS `mm` units in browser for print accuracy

#### Implementation
```typescript
// shared-constants.ts
export const CARD_WIDTH_MM = 85;
export const CARD_HEIGHT_MM = 55;
export const YEAR_FONT_SIZE_MM = 8;  // Maps to 24pt
export const ARTIST_FONT_SIZE_MM = 3; // Maps to 9pt
```

```svelte
<style>
  .year {
    font-size: 8mm; /* Matches QuestPDF 24pt */
  }
</style>
```

#### Challenges
- Browser `mm` units are approximate, not print-accurate
- Font rendering differs between browsers and QuestPDF
- Must manually keep two codebases in sync
- Text wrapping/overflow may differ

#### Pros
- **No new dependencies**: Pure CSS solution
- **Fast preview**: Local rendering, no network
- **Incremental**: Can gradually improve accuracy

#### Cons
- **NOT pixel-perfect**: Browser and PDF will always differ slightly
- **Maintenance burden**: Must sync two codebases
- **Font differences**: Cannot be resolved without same renderer
- **Testing overhead**: Must visually compare on every change

#### Scoring
| Criterion | Score | Notes |
|-----------|-------|-------|
| Performance | 5 | Pure client-side, no network |
| Simplicity | 3 | Two codebases to maintain |
| Pixel-Perfect | 1 | Inherently different renderers |
| **Total** | **9** | |

---

## Comparative Analysis

### Summary Table

| Option | Performance | Simplicity | Pixel-Perfect | Total | Recommendation |
|--------|-------------|------------|---------------|-------|----------------|
| A: QuestPDF Images | 3 | 5 | 5 | 13 | **RECOMMENDED** |
| B: SVG Generation | 5 | 2 | 2 | 9 | Not recommended |
| C: SkiaSharp WASM | 2 | 1 | 4 | 7 | Not recommended |
| D: HTML-to-PDF | 1 | 2 | 5 | 8 | Not recommended |
| E: CSS Sync | 5 | 3 | 1 | 9 | Not recommended |

### Decision Matrix by Constraint Priority

If user prioritized **Performance** only: Option E (CSS Sync) or B (SVG)
If user prioritized **Simplicity** only: Option A (QuestPDF Images)
If user prioritized **Pixel-Perfect** only: Option A or D (both score 5)
If **All constraints equal** (current case): **Option A (QuestPDF Images)** wins

---

## Recommended Approach: Option A - QuestPDF Server-Side Images

### Why This Option?

1. **Already Partially Implemented**: `CardDesigner.cs` already has `GenerateFrontCardImage()` and `GenerateBackCardImage()` methods

2. **Zero Visual Drift**: Same QuestPDF code generates preview and PDF - no possibility of divergence

3. **Minimal Changes**:
   - Add 2 API endpoints for card images
   - Replace Svelte components with `<img>` tags
   - Add caching layer

4. **Proven Technology**: QuestPDF image generation is mature and well-documented

### Implementation Plan (High-Level)

#### Phase 26.1: API Endpoints
1. Add `GET /api/card-preview/front` endpoint
2. Add `GET /api/card-preview/back` endpoint
3. Implement in-memory caching with hash-based keys

#### Phase 26.2: Frontend Migration
1. Replace `CardFront.svelte` with image-based component
2. Replace `CardBack.svelte` with image-based component
3. Implement image preloading for carousel

#### Phase 26.3: Optimization
1. Add cache headers for browser caching
2. Implement lazy loading for off-screen cards
3. Consider WebP format for smaller payloads

### Expected Latency Analysis

| Scenario | Latency | Notes |
|----------|---------|-------|
| First card load | 100-150ms | Network + generation |
| Cached card load | 20-50ms | Network only |
| Card flip (pre-fetched) | <16ms | CSS only |
| Card flip (cold) | 100-150ms | Need to fetch back |
| Color change | 100-150ms | Cache miss, regenerate |

### Handling Dynamic Color Updates

When user changes genre color:
1. Invalidate cache for affected cards
2. Regenerate preview images on next request
3. Use loading spinner during regeneration

---

## Alternative Consideration: Hybrid Approach

If latency for color changes is unacceptable, consider a **hybrid approach**:

1. **Static elements** (QR code, layout): Server-generated images
2. **Dynamic elements** (background color): CSS overlay

```svelte
<div class="card-container" style="background-color: {genreColor}">
  <img src="/api/card-preview/back/{cardId}" alt="Card back" />
</div>
```

This adds complexity but allows instant color feedback while maintaining pixel-perfect layout.

---

## References

- [QuestPDF Generating Output Documentation](https://www.questpdf.com/concepts/generating-output.html)
- [QuestPDF Image Generation Settings](https://www.questpdf.com/api-reference/image.html)
- [QuestPDF Memory Issues with GenerateImages](https://github.com/QuestPDF/QuestPDF/issues/968)
- [QuestPDF Performance Benchmarks](https://github.com/QuestPDF/QuestPDF/pull/1027)
- [Base64 Encoding Performance Anti-Patterns](https://calendar.perfplanet.com/2018/performance-anti-patterns-base64-encoding/)
- [SkiaSharp WebAssembly Support](https://skiasharp.com/)
- [HTML to PDF C# Comparison 2025](https://dev.to/mhamzap10/questpdf-html-to-pdf-c-alternatives-for-net-developers-22gl)

---

## Next Steps

1. **User Decision**: Present this research and confirm Option A recommendation
2. **Plan Phase**: Create detailed implementation plan for 26.1 (API Endpoints)
3. **Prototype**: Build minimal endpoint to validate performance assumptions
4. **Iterate**: Measure actual latency and adjust caching strategy
