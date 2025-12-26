# Phase 27: Simplify Color System - Context

## User Vision

The user wants to simplify the color system by removing unnecessary complexity. The current implementation offers multiple color palettes and per-genre color customization, but this flexibility is not needed. The Spotify palette should be the default and only option.

## What to Remove

### 1. Palette Selection UI
- **File:** `web/src/lib/ColorSettings/ColorPalettes.svelte`
- Remove the palette selection grid that lets users choose between Spotify, Neon, Pastel, and Grayscale palettes
- Remove the `onSelectPalette` callback and palette application logic

### 2. Built-in Genres List
- The hardcoded list of 35 genres in `ColorPalettes.svelte` and `GenreColorPicker.svelte` should be removed
- Genres should come from actual song data only, not a predefined list

### 3. Color Customization Features
- Remove the ability to change individual genre colors via the color picker
- Remove localStorage persistence for custom colors (`hitster-genre-colors` key)
- Remove `setGenreColor`, `resetGenreColors`, `applyPalette` functions from the store

## What to Keep

### 1. Genre Colors View (Read-Only)
- Keep `GenreColorPicker.svelte` but transform it into a read-only reference view
- Users should be able to see what color is assigned to each genre in their song list
- No editing capability - just visual reference

### 2. DEFAULT_GENRE_COLORS
- Keep the Spotify palette colors as the single source of truth
- These are defined in `cardCustomization.svelte.ts` as DEFAULT_GENRE_COLORS (35 genres)
- Keep `getGenreColor()` function for looking up colors

## What to Hardcode

### Spotify Palette (Default Genre Colors)
The following colors will be the only palette used, as defined in `DEFAULT_GENRE_COLORS`:
- Rock: #E63946, Pop: #FF69B4, Hip-Hop: #FFD700, R&B: #9B59B6
- Country: #D2691E, Jazz: #6B5B95, Blues: #4169E1, Electronic: #00CED1
- Dance: #FF1493, House: #32CD32, Techno: #008B8B, Classical: #1E3A5F
- Reggae: #228B22, Soul: #8B0000, Funk: #FF8C00, Disco: #DA70D6
- Metal: #2F4F4F, Punk: #FF00FF, Alternative: #2E8B57, Indie: #DAA520
- Folk: #808000, Latin: #FF6347, Rap: #B8860B, Gospel: #FFE4B5
- World: #8B4513, Ambient: #87CEEB, New Wave: #7B68EE, Grunge: #556B2F
- Ska: #20B2AA, Synthpop: #FF1493
- French genres: Chanson: #0055A4, Variete Francaise: #3B5998, French Pop: #FF69B4, French Hip-Hop: #FFD700, Musette: #EF4135

Unknown genres will fall back to gray (#808080).

## Current Architecture

### Files to Modify
1. `web/src/lib/ColorSettings/ColorPalettes.svelte` - Remove entirely or gut
2. `web/src/lib/ColorSettings/GenreColorPicker.svelte` - Simplify to read-only view
3. `web/src/lib/stores/cardCustomization.svelte.ts` - Remove customization functions, keep getGenreColor

### Current Flow
1. User navigates to Card Preview step
2. GenreColorPicker shows ColorPalettes component (palette selection)
3. User can select a palette OR click individual genres to customize
4. Colors are stored in localStorage and applied to cards

### Simplified Flow (Target)
1. User navigates to Card Preview step
2. Read-only Genre Colors view shows assigned colors for genres in their songs
3. Colors use DEFAULT_GENRE_COLORS (Spotify palette) - no customization
4. No localStorage needed for colors

## Success Criteria

- [ ] ColorPalettes component removed or replaced with static display
- [ ] GenreColorPicker is read-only (no click handlers, no color picker)
- [ ] localStorage for genre colors removed
- [ ] Only genres from actual songs are shown (no predefined list)
- [ ] Cards render with Spotify palette colors
- [ ] UI still shows genre color assignments as a reference
