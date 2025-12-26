# Phase 26: Unified Rendering - Context

## Phase Goal

Research and implement a unified card rendering approach so that:
- Preview in browser looks exactly like the exported PDF
- Same rendering logic is used for both
- Eliminates the current mismatch between preview and final output

## Current State Analysis

### Browser Preview (Svelte/CSS)
- **Files**: `CardFront.svelte`, `CardBack.svelte`
- **Technology**: HTML + CSS with inline styles
- **Layout**: Uses flexbox, aspect-ratio CSS, variable padding
- **Fonts**: System fonts with pixel/rem sizing
- **Colors**: Dynamic text color calculation based on background brightness
- **Features**: Card flip animation, responsive sizing

### PDF Export (QuestPDF/.NET)
- **Files**: `CardDesigner.cs`, `PdfExporter.cs`
- **Technology**: QuestPDF library (C#)
- **Layout**: Uses millimeter measurements (85x55mm card size)
- **Fonts**: QuestPDF default fonts with point sizes
- **Colors**: Hardcoded QuestPDF Colors (Grey.Darken2, etc.)
- **Features**: Grid layout for printing, cutting lines, duplex support

### Key Differences
1. **Font sizes**: Preview uses px (56px for year), PDF uses pt (24pt for year)
2. **Spacing**: Preview uses px/rem, PDF uses mm
3. **Text colors**: Preview calculates from background, PDF uses hardcoded grey values
4. **Card front**: Preview shows album art, PDF shows QR code
5. **Overall styling**: Significantly different visual appearance

## User Requirements (from discuss-milestone)

1. **Research all options** - Explore multiple approaches
2. **Present pros/cons** - User will make final decision
3. **Implement chosen solution** - After user decides

## Constraints to Consider

Based on the project goals:
- **Pixel-perfect accuracy**: Preview must match PDF exactly
- **Print quality**: Cards must remain print-ready (credit-card size, proper margins)
- **Performance**: Preview should be fast/responsive in browser
- **Simplicity**: Prefer simpler solutions over complex ones (v2.8 theme is "Simplification")
- **Maintenance**: Single source of truth is easier to maintain

## Options to Research

### Option 1: Server-Side Image Generation
- Generate PNG images on server using QuestPDF
- Serve images to browser for preview
- Same rendering code for preview and PDF

### Option 2: Server-Side SVG Generation
- Generate SVG on server
- Embed in browser for preview
- Convert to PDF (or use QuestPDF with SVG output)

### Option 3: Shared Canvas/SVG in Browser
- Render cards using Canvas or SVG in browser
- Export to PDF using same rendering definitions
- Would require rewriting QuestPDF logic in JavaScript

### Option 4: QuestPDF-Driven Everything
- Use QuestPDF to generate preview images
- Keep existing PDF export
- Browser displays QuestPDF-generated images

### Option 5: Hybrid - CSS-to-PDF Library
- Use a library that converts HTML/CSS to PDF
- Preview remains in browser
- PDF generation from same HTML

## Questions for User

Before proceeding with research, I need clarification on one key aspect:

**What constraints matter most for the unified rendering?**

1. **Performance** - Fast preview updates as user browses cards
2. **Simplicity** - Easiest to implement and maintain
3. **Pixel-perfect accuracy** - Zero visual difference between preview and PDF
4. **Offline capability** - Preview works without server calls (for future consideration)

This will help prioritize which options to research more deeply.

## Next Steps

1. Get user input on constraints priority
2. Research each option's feasibility
3. Create detailed pros/cons analysis
4. Present to user for decision
5. Implement chosen solution
