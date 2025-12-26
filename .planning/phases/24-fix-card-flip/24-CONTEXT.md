# Phase 24: Fix Card Flip - Context

## Vision
The flip button in CardControls should flip the currently visible card in the carousel. When a user clicks the flip button, the card should animate from front to back (or back to front).

## Essential
- Button click must trigger flip animation on the current card
- Fix the broken click handler connection
- Maintain existing functionality (clicking card directly to flip still works)

## Bug Description
- Flip button click handler is not working
- Button hover animation works correctly (button is rendering)
- The issue is likely in the event handler binding or the onFlip callback chain

## Boundaries (Out of Scope)
- Don't change the hover animation (it works)
- Don't change the card flip animation itself
- Don't change the click-on-card-to-flip behavior
- This is a bug fix, not a feature change

## Notes
- Check CardControls.svelte for the flip button onClick handler
- Trace the onFlip prop through to App.svelte
- Verify the flip function is being called with correct card ID
