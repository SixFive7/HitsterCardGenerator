# Phase 27: Simplify Color System - Execution Plan

**Created:** 2025-12-26
**Type:** Simplification/Removal
**Estimated Duration:** 10-15 minutes

## Objective

Remove color customization features and hardcode Spotify palette as the only option. Keep genre colors as a read-only reference view.

## Pre-Conditions

- [x] Phase 26 (Unified Rendering) complete
- [x] Color system currently supports 4 palettes (Spotify, Neon, Pastel, Grayscale)
- [x] GenreColorPicker allows per-genre color editing via color picker
- [x] Colors persist to localStorage (`hitster-genre-colors`)

## Current State Analysis

### Files to Modify/Remove

| File | Action | Reason |
|------|--------|--------|
| `web/src/lib/ColorSettings/ColorPalettes.svelte` | DELETE | Palette selection no longer needed |
| `web/src/lib/ColorSettings/GenreColorPicker.svelte` | SIMPLIFY | Convert to read-only view |
| `web/src/lib/stores/cardCustomization.svelte.ts` | SIMPLIFY | Remove customization functions |
| `web/package.json` | MODIFY | Remove `svelte-awesome-color-picker` dependency |

### Dependencies

- `GenreColorPicker` is imported in `App.svelte` (line 8, used line 562)
- `DEFAULT_GENRE_COLORS` is used in `PlaylistBuilder.svelte` for genre dropdown options
- `getGenreColor` is used in `App.svelte` and `CardCarousel.svelte`

---

## Tasks

### Task 1: Simplify Store (cardCustomization.svelte.ts)

**Goal:** Remove customization functions, keep only read-only color lookup

**Changes:**
1. Remove `STORAGE_KEY_COLORS` constant
2. Remove `loadGenreColors()` function
3. Remove `saveGenreColors()` function
4. Remove `setGenreColor()` export
5. Remove `resetGenreColors()` export
6. Remove `applyPalette()` export
7. Remove `genreColors` reactive state
8. Simplify `getGenreColor()` to look up directly from `DEFAULT_GENRE_COLORS`
9. Keep `DEFAULT_GENRE_COLORS` export (used by PlaylistBuilder)
10. Keep `currentCardIndex` state and `getCardCustomizationState()` (used for carousel)

**Verification:**
- TypeScript compiles without errors
- App runs and cards display with correct colors

---

### Task 2: Simplify GenreColorPicker and Remove ColorPalettes

**Goal:** Convert to read-only reference view, delete palette selection

**Changes:**

**2a. Delete ColorPalettes.svelte:**
- Remove file: `web/src/lib/ColorSettings/ColorPalettes.svelte`

**2b. Simplify GenreColorPicker.svelte:**
1. Remove `import ColorPicker from 'svelte-awesome-color-picker'`
2. Remove `import ColorPalettes from './ColorPalettes.svelte'`
3. Remove `setGenreColor` from imports (keep only `getGenreColor`)
4. Remove `selectedGenre` and `selectedColor` state
5. Remove `selectGenre()` function
6. Remove `handleColorChange()` function
7. Remove `handlePaletteSelect()` function
8. Remove `getPaletteData()` function (entire 50+ line function)
9. Remove `<ColorPalettes>` component usage
10. Remove color picker section (`{#if selectedGenre}` block)
11. Change genre buttons to non-interactive divs (remove `onclick`, cursor)
12. Update styles: remove hover effects, remove `.selected` class

**2c. Update App.svelte imports:**
- Remove `setGenreColor` if imported (check imports)

**2d. Remove color picker dependency:**
- Run `npm uninstall svelte-awesome-color-picker` in web/ directory

**Verification:**
- GenreColorPicker displays as read-only grid of genre colors
- No click interactions on genres
- TypeScript compiles without errors

---

### Task 3: Clean Up and Verify

**Goal:** Remove any remaining localStorage usage, test the application

**Changes:**
1. Clear any old localStorage entries (document in README if needed)
2. Test CSV upload flow with genre colors
3. Test playlist builder genre dropdown still works
4. Test card preview with Spotify colors
5. Verify PDF export still works

**Visual Verification (Chrome DevTools MCP):**
- Take screenshot of Card Preview step
- Verify genre color reference shows correctly
- Verify colors appear on cards

---

## Success Criteria

- [ ] ColorPalettes.svelte deleted
- [ ] GenreColorPicker is read-only (no click handlers, no color picker)
- [ ] Store only has `DEFAULT_GENRE_COLORS`, `getGenreColor()`, and carousel state
- [ ] `svelte-awesome-color-picker` removed from package.json
- [ ] localStorage for genre colors no longer used
- [ ] Cards render with Spotify palette colors (DEFAULT_GENRE_COLORS)
- [ ] PlaylistBuilder genre dropdown still works
- [ ] Application compiles and runs without errors

## Post-Conditions

- Color system is simplified to single hardcoded palette
- No user-facing color customization
- Genre colors view is read-only reference
- Reduced bundle size (removed color picker library)
