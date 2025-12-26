# Phase 28: Card Redesign - Context

## Overview

Redesign the card layout for both front and back faces using the unified QuestPDF rendering system (implemented in Phase 26). The new design centers key elements and adds genre text to the front, album art to the back, and uses faded backgrounds behind text for readability.

## Current State

- **Card rendering:** Unified via QuestPDF (CardDesigner.cs) - preview and PDF use same code
- **Card dimensions:** 85mm x 55mm (credit card size, landscape orientation)
- **Color system:** Hardcoded Spotify palette per genre (Phase 27)
- **Front card (current):** Genre background color + centered QR code (45mm)
- **Back card (current):** Genre background color + artist (top) + year (center, large) + title + genre (bottom)
- **Album art:** Available via `albumImageUrl` in SpotifyMatch but not currently used in cards
- **Flip animation:** CSS 3D transform, horizontal axis, 0.6s duration

## New Design Specification

### Front of Card

```
+----------------------------------+
|                                  |
|        [GENRE BACKGROUND]        |
|                                  |
|          +--------+              |
|          |   QR   |              |
|          |  CODE  |              |
|          +--------+              |
|                                  |
|           "Genre"                |
|                                  |
+----------------------------------+
```

- **Background:** Full genre color fill
- **QR code:** Centered horizontally and vertically, sized appropriately
- **Genre text:** Below QR code, centered, readable against genre color

### Back of Card

```
+----------------------------------+
| [  Year | Genre  ]    <- top bar |
|----------------------------------|
|                                  |
|        +------------+            |
|        |            |            |
|        | Album Art  |            |
|        |            |            |
|        +------------+            |
|                                  |
|----------------------------------|
| [Artist - Title - Album] <- btm |
+----------------------------------+
```

- **Background:** Full genre color fill
- **Top bar:** Year and genre text on semi-transparent background strip (NOT overlaid on album art)
- **Album art:** Centered, appropriately sized (not full bleed)
- **Bottom bar:** Artist, title, album text on semi-transparent background strip
- **Text readability:** Semi-transparent/faded background behind text strips for contrast

### Visual Design Goals

1. **Clean and polished:** Professional, game-quality card appearance
2. **Readable text:** Semi-transparent backgrounds ensure text is visible regardless of genre color brightness
3. **Centered focus:** QR code (front) and album art (back) are the visual focal points
4. **Genre identity:** Background color provides instant genre recognition
5. **Information hierarchy:** Back card shows year prominently with supporting details

### Animation

- **Type:** Horizontal flip (rotate around Y-axis)
- **Duration:** 0.6s (current timing is good)
- **Easing:** cubic-bezier(0.4, 0.0, 0.2, 1) (smooth deceleration)

## Technical Requirements

### Data Flow Changes

1. **CardPreviewRequest** needs `albumImageUrl` property
2. **CardData model** needs `AlbumImageUrl` property
3. **CardDesigner** needs to fetch/embed album art image
4. **Frontend** needs to pass `albumImageUrl` in card preview requests

### QuestPDF Implementation Notes

- Use `Image()` component for album art (fetch from URL and cache)
- Use `Background()` with alpha for semi-transparent strips
- Consider image caching for album art (avoid re-fetching)
- Handle missing album art gracefully (fallback placeholder)

### Files to Modify

| File | Changes |
|------|---------|
| `Models/CardData.cs` | Add `AlbumImageUrl` and `AlbumName` properties |
| `Services/CardDesigner.cs` | Implement new front/back designs with album art |
| `Endpoints/CardPreviewEndpoints.cs` | Add `albumImageUrl` and `albumName` to request |
| `web/src/lib/CardPreview/CardCarousel.svelte` | Pass `albumImageUrl` and `albumName` to card components |
| `web/src/lib/CardPreview/CardFront.svelte` | Pass `albumImageUrl` to API request |
| `web/src/lib/CardPreview/CardBack.svelte` | Pass `albumImageUrl` and `albumName` to API request |
| `Services/PdfExporter.cs` | Update to use new card designs (should work automatically via CardDesigner) |

## Success Criteria

1. Front card shows: genre background + centered QR code + genre text below
2. Back card shows: genre background + year/genre top bar + centered album art + artist/title/album bottom bar
3. Text has semi-transparent background strips for readability
4. Album art displays correctly (fetched from Spotify URL)
5. Flip animation works smoothly
6. PDF export matches preview exactly
7. Cards look polished and professional

## Verification Approach

Use Chrome DevTools MCP to:
1. Take screenshots of card front and back
2. Verify layout matches specifications
3. Test flip animation
4. Compare preview to PDF export
5. Iterate on design until polished (Phase 29 handles final polish)

## Dependencies

- Phase 26 (Unified Rendering) - Complete
- Phase 27 (Simplify Color System) - Complete

## Open Questions

None - design specs are comprehensive. Implementation can proceed.
