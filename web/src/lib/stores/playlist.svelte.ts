/**
 * Playlist store using Svelte 5 runes
 * Manages playlist tracks with API synchronization
 */

import type { SearchResult, PlaylistTrack, PlaylistDetail, TrackDto } from '../types'
import { getPlaylistDetail, addTrackToPlaylist, removeTrackFromPlaylist, type AddTrackRequest } from '../api'

// Extended PlaylistTrack with API track ID for removal capability
interface PlaylistTrackWithId extends PlaylistTrack {
  id?: string  // API track ID (from LiteDB)
}

// Create the store state
let tracks = $state<PlaylistTrackWithId[]>([])
let currentPlaylistId = $state<string | null>(null)
let isLoading = $state<boolean>(false)

/**
 * Converts a TrackDto from API to PlaylistTrack format
 */
function trackDtoToPlaylistTrack(dto: TrackDto): PlaylistTrackWithId {
  return {
    id: dto.id,
    trackId: dto.spotifyId,  // Spotify track ID
    trackName: dto.title,
    artistName: dto.artist,
    albumName: '',  // Not stored in API, use empty string
    albumImageUrl: dto.albumArtUrl ?? '',
    spotifyUrl: `https://open.spotify.com/track/${dto.spotifyId}`,
    releaseYear: dto.year,
    genre: dto.genre
  }
}

/**
 * Loads tracks from API for a specific playlist
 */
export async function loadTracksFromPlaylist(playlistId: string): Promise<void> {
  if (!playlistId) {
    tracks = []
    currentPlaylistId = null
    return
  }

  isLoading = true
  try {
    const detail = await getPlaylistDetail(playlistId)
    tracks = detail.tracks.map(trackDtoToPlaylistTrack)
    currentPlaylistId = playlistId
  } catch (error) {
    console.error('Failed to load playlist tracks:', error)
    tracks = []
    currentPlaylistId = null
  } finally {
    isLoading = false
  }
}

/**
 * Adds a track to the playlist
 * Converts SearchResult to PlaylistTrack with default genre "Pop"
 * Saves to API if playlist is selected
 */
export async function addTrack(result: SearchResult, playlistId?: string): Promise<void> {
  const targetPlaylistId = playlistId ?? currentPlaylistId

  // Check for duplicates by Spotify track ID
  if (tracks.some(track => track.trackId === result.trackId)) {
    return
  }

  // If we have a playlist ID, save to API first
  if (targetPlaylistId) {
    try {
      const trackRequest: AddTrackRequest = {
        spotifyId: result.trackId,
        title: result.trackName,
        artist: result.artistName,
        year: result.releaseYear,
        genre: 'Pop',  // Default genre for Spotify search results
        albumArtUrl: result.albumImageUrl
      }

      const response = await addTrackToPlaylist(targetPlaylistId, trackRequest)

      // Add to local store with API track ID
      const playlistTrack: PlaylistTrackWithId = {
        id: response.track.id,
        trackId: result.trackId,
        trackName: result.trackName,
        artistName: result.artistName,
        albumName: result.albumName,
        albumImageUrl: result.albumImageUrl,
        spotifyUrl: result.spotifyUrl,
        releaseYear: result.releaseYear,
        genre: 'Pop'
      }

      tracks = [...tracks, playlistTrack]
    } catch (error) {
      console.error('Failed to add track to playlist:', error)
    }
  } else {
    // No playlist selected, just add to local store (session-only)
    const playlistTrack: PlaylistTrackWithId = {
      trackId: result.trackId,
      trackName: result.trackName,
      artistName: result.artistName,
      albumName: result.albumName,
      albumImageUrl: result.albumImageUrl,
      spotifyUrl: result.spotifyUrl,
      releaseYear: result.releaseYear,
      genre: 'Pop'
    }

    tracks = [...tracks, playlistTrack]
  }
}

/**
 * Adds a track with full data (used for CSV import)
 * Saves to API if playlist is selected
 */
export async function addTrackWithData(
  track: {
    spotifyId: string
    title: string
    artist: string
    year: number
    genre: string
    albumImageUrl?: string
    albumName?: string
    spotifyUrl?: string
  },
  playlistId?: string
): Promise<void> {
  const targetPlaylistId = playlistId ?? currentPlaylistId

  // Check for duplicates by Spotify track ID
  if (tracks.some(t => t.trackId === track.spotifyId)) {
    return
  }

  // If we have a playlist ID, save to API first
  if (targetPlaylistId) {
    try {
      const trackRequest: AddTrackRequest = {
        spotifyId: track.spotifyId,
        title: track.title,
        artist: track.artist,
        year: track.year,
        genre: track.genre,
        albumArtUrl: track.albumImageUrl
      }

      const response = await addTrackToPlaylist(targetPlaylistId, trackRequest)

      // Add to local store with API track ID
      const playlistTrack: PlaylistTrackWithId = {
        id: response.track.id,
        trackId: track.spotifyId,
        trackName: track.title,
        artistName: track.artist,
        albumName: track.albumName ?? '',
        albumImageUrl: track.albumImageUrl ?? '',
        spotifyUrl: track.spotifyUrl ?? `https://open.spotify.com/track/${track.spotifyId}`,
        releaseYear: track.year,
        genre: track.genre
      }

      tracks = [...tracks, playlistTrack]
    } catch (error) {
      console.error('Failed to add track to playlist:', error)
    }
  } else {
    // No playlist selected, just add to local store (session-only)
    const playlistTrack: PlaylistTrackWithId = {
      trackId: track.spotifyId,
      trackName: track.title,
      artistName: track.artist,
      albumName: track.albumName ?? '',
      albumImageUrl: track.albumImageUrl ?? '',
      spotifyUrl: track.spotifyUrl ?? `https://open.spotify.com/track/${track.spotifyId}`,
      releaseYear: track.year,
      genre: track.genre
    }

    tracks = [...tracks, playlistTrack]
  }
}

/**
 * Removes a track from the playlist by Spotify trackId
 * Also removes from API if playlist is selected
 */
export async function removeTrack(trackId: string): Promise<void> {
  const track = tracks.find(t => t.trackId === trackId)

  if (!track) return

  // If we have the API track ID and a playlist, remove from API
  if (track.id && currentPlaylistId) {
    try {
      await removeTrackFromPlaylist(currentPlaylistId, track.id)
    } catch (error) {
      console.error('Failed to remove track from playlist:', error)
      return  // Don't remove from local store if API fails
    }
  }

  tracks = tracks.filter(t => t.trackId !== trackId)
}

/**
 * Updates the genre for a specific track
 * Note: Genre updates are local-only for now (not persisted to API)
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
 * Clears the entire playlist (local store only)
 * Does not delete tracks from API - use for UI reset
 */
export function clearPlaylist() {
  tracks = []
  currentPlaylistId = null
}

/**
 * Sets the current playlist ID without loading tracks
 * Used when changing playlists
 */
export function setCurrentPlaylistId(id: string | null) {
  currentPlaylistId = id
}

/**
 * Gets the current playlist ID
 */
export function getCurrentPlaylistId(): string | null {
  return currentPlaylistId
}

/**
 * Export reactive state
 */
export function getPlaylistState() {
  return {
    get tracks() { return tracks as PlaylistTrack[] },
    get isLoading() { return isLoading },
    get currentPlaylistId() { return currentPlaylistId }
  }
}
