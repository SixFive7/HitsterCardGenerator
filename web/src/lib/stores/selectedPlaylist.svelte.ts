/**
 * Selected playlist store using Svelte 5 runes
 * Manages the currently selected playlist ID, persisted to localStorage
 */

const SELECTED_PLAYLIST_KEY = 'hitster-selected-playlist'

// Initialize from localStorage
function initSelectedPlaylistId(): string | null {
  if (typeof window === 'undefined') {
    return null
  }
  return localStorage.getItem(SELECTED_PLAYLIST_KEY)
}

// Store state
let selectedPlaylistId = $state<string | null>(initSelectedPlaylistId())

/**
 * Get the current selected playlist ID (non-reactive)
 */
export function getSelectedPlaylistId(): string | null {
  return selectedPlaylistId
}

/**
 * Set the selected playlist ID and persist to localStorage
 */
export function setSelectedPlaylistId(id: string | null) {
  selectedPlaylistId = id

  if (typeof window !== 'undefined') {
    if (id) {
      localStorage.setItem(SELECTED_PLAYLIST_KEY, id)
    } else {
      localStorage.removeItem(SELECTED_PLAYLIST_KEY)
    }
  }
}

/**
 * Clear the selected playlist
 */
export function clearSelectedPlaylist() {
  setSelectedPlaylistId(null)
}

/**
 * Get reactive selected playlist state
 */
export function getSelectedPlaylistState() {
  return {
    get selectedPlaylistId() { return selectedPlaylistId }
  }
}
