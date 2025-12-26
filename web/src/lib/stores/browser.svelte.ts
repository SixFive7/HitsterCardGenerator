/**
 * Browser identity store using Svelte 5 runes
 * Manages a persistent browser UUID stored in localStorage
 */

const BROWSER_ID_KEY = 'hitster-browser-id'

// Initialize browser ID from localStorage or generate new one
function initBrowserId(): string {
  if (typeof window === 'undefined') {
    return ''
  }

  let browserId = localStorage.getItem(BROWSER_ID_KEY)

  if (!browserId) {
    browserId = crypto.randomUUID()
    localStorage.setItem(BROWSER_ID_KEY, browserId)
  }

  return browserId
}

// Store state
let browserId = $state<string>(initBrowserId())

/**
 * Get the browser ID for API calls
 * This is a non-reactive getter for use in API functions
 */
export function getBrowserId(): string {
  // Ensure we have a browser ID (handles SSR/hydration)
  if (!browserId && typeof window !== 'undefined') {
    browserId = initBrowserId()
  }
  return browserId
}

/**
 * Get reactive browser state
 */
export function getBrowserState() {
  return {
    get browserId() { return browserId }
  }
}
