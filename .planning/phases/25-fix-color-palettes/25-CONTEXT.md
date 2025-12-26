# Phase 25: Fix Color Palettes - Context

## Vision
When a user selects a color palette, the card preview should immediately update to show the new colors. The genre-based color scheme should be applied to the cards in the carousel.

## Essential
- Palette selection must update the card preview in real-time
- Fix the connection between palette selection and card rendering
- Maintain the existing palette UI (it works, just doesn't apply)

## Bug Description
- Color palette picker UI works (can select palettes)
- But card preview doesn't update to reflect the selection
- The issue is likely in the reactive binding between the palette store and card components

## Boundaries (Out of Scope)
- Don't change the palette picker UI (it works)
- Don't change the available color palettes
- Don't change the card layout/design
- This is a bug fix, not a feature change

## Notes
- Check cardCustomization.svelte.ts for palette state management
- Trace how genreColors are passed to card components
- Look at CardFront.svelte and CardBack.svelte for color application
- Check if the reactive binding ($state) is properly triggering updates
