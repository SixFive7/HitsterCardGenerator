/**
 * Playlist store using Svelte 5 runes
 * Manages playlist tracks for session-based playlist building (no localStorage)
 */

import type { SearchResult, PlaylistTrack } from '../types'

// Create the store state (session-only, no localStorage)
let tracks = $state<PlaylistTrack[]>([])

/**
 * Adds a track to the playlist
 * Converts SearchResult to PlaylistTrack with default genre "Pop"
 * Prevents duplicates by checking trackId
 */
export function addTrack(result: SearchResult) {
  // Check for duplicates
  if (tracks.some(track => track.trackId === result.trackId)) {
    return
  }

  // Convert SearchResult to PlaylistTrack with default genre
  const playlistTrack: PlaylistTrack = {
    ...result,
    genre: 'Pop'
  }

  tracks.push(playlistTrack)
  // Trigger reactivity
  tracks = [...tracks]
}

/**
 * Removes a track from the playlist by trackId
 */
export function removeTrack(trackId: string) {
  tracks = tracks.filter(track => track.trackId !== trackId)
}

/**
 * Updates the genre for a specific track
 */
export function updateTrackGenre(trackId: string, genre: string) {
  const track = tracks.find(t => t.trackId === trackId)
  if (track) {
    track.genre = genre
    // Trigger reactivity
    tracks = [...tracks]
  }
}

/**
 * Clears the entire playlist
 */
export function clearPlaylist() {
  tracks = []
}

/**
 * Export reactive state
 */
export function getPlaylistState() {
  return {
    get tracks() { return tracks }
  }
}
