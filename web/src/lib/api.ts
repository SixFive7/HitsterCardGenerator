import type { CsvUploadResponse, MatchResponse, Song, ExportRequest, SearchResult, Playlist, PlaylistDetail, TrackDto } from './types'
import { getBrowserId } from './stores/browser.svelte'

/**
 * Get headers with browser ID for API calls
 */
function getHeaders(contentType?: string): HeadersInit {
  const headers: HeadersInit = {
    'X-Browser-Id': getBrowserId()
  }
  if (contentType) {
    headers['Content-Type'] = contentType
  }
  return headers
}

export async function fetchHealth(): Promise<{ status: string; timestamp: string }> {
  const response = await fetch('/api/health', {
    headers: getHeaders()
  })
  return response.json()
}

export async function uploadCsv(file: File): Promise<CsvUploadResponse> {
  const formData = new FormData()
  formData.append('file', file)

  const response = await fetch('/api/csv/upload', {
    method: 'POST',
    headers: { 'X-Browser-Id': getBrowserId() },
    body: formData,
  })

  if (!response.ok) {
    const error = await response.json()
    throw new Error(error.errorSummary || 'Failed to upload CSV')
  }

  return response.json()
}

export async function matchSongs(songs: Song[]): Promise<MatchResponse> {
  const response = await fetch('/api/match', {
    method: 'POST',
    headers: getHeaders('application/json'),
    body: JSON.stringify({
      songs: songs.map(s => ({
        title: s.title,
        artist: s.artist,
        year: s.year,
        genre: s.genre
      }))
    })
  })
  if (!response.ok) throw new Error('Failed to match songs')
  return response.json()
}

export async function exportPdf(request: ExportRequest): Promise<Blob> {
  const response = await fetch('/api/export', {
    method: 'POST',
    headers: getHeaders('application/json'),
    body: JSON.stringify(request)
  })
  if (!response.ok) throw new Error('Failed to export PDF')
  return response.blob()
}

export async function searchSpotify(query: string): Promise<SearchResult[]> {
  // Return empty array for empty queries (don't call API)
  if (!query.trim()) {
    return []
  }

  const response = await fetch(`/api/search?q=${encodeURIComponent(query)}`, {
    headers: getHeaders()
  })
  if (!response.ok) throw new Error('Failed to search Spotify')
  return response.json()
}

// Playlist API functions

export async function fetchPlaylists(): Promise<Playlist[]> {
  const response = await fetch('/api/playlists', {
    headers: getHeaders()
  })
  if (!response.ok) throw new Error('Failed to fetch playlists')
  return response.json()
}

export async function createPlaylist(name: string): Promise<Playlist> {
  const response = await fetch('/api/playlists', {
    method: 'POST',
    headers: getHeaders('application/json'),
    body: JSON.stringify({ name })
  })
  if (!response.ok) throw new Error('Failed to create playlist')
  return response.json()
}

export async function getPlaylist(id: string): Promise<Playlist> {
  const response = await fetch(`/api/playlists/${encodeURIComponent(id)}`, {
    headers: getHeaders()
  })
  if (!response.ok) throw new Error('Failed to get playlist')
  return response.json()
}

export async function getPlaylistDetail(id: string): Promise<PlaylistDetail> {
  const response = await fetch(`/api/playlists/${encodeURIComponent(id)}`, {
    headers: getHeaders()
  })
  if (!response.ok) throw new Error('Failed to get playlist')
  return response.json()
}

export interface AddTrackRequest {
  spotifyId: string
  title: string
  artist: string
  year: number
  genre: string
  albumArtUrl?: string
}

export interface AddTrackResponse {
  trackId: string
  playlistId: string
  track: TrackDto
}

export async function addTrackToPlaylist(
  playlistId: string,
  track: AddTrackRequest
): Promise<AddTrackResponse> {
  const response = await fetch(`/api/playlists/${encodeURIComponent(playlistId)}/tracks`, {
    method: 'POST',
    headers: getHeaders('application/json'),
    body: JSON.stringify({
      spotifyId: track.spotifyId,
      title: track.title,
      artist: track.artist,
      year: track.year,
      genre: track.genre,
      albumArtUrl: track.albumArtUrl
    })
  })
  if (!response.ok) throw new Error('Failed to add track to playlist')
  return response.json()
}

export async function removeTrackFromPlaylist(playlistId: string, trackId: string): Promise<void> {
  const response = await fetch(`/api/playlists/${encodeURIComponent(playlistId)}/tracks/${encodeURIComponent(trackId)}`, {
    method: 'DELETE',
    headers: getHeaders()
  })
  if (!response.ok) throw new Error('Failed to remove track from playlist')
}

export async function renamePlaylist(id: string, name: string): Promise<Playlist> {
  const response = await fetch(`/api/playlists/${encodeURIComponent(id)}`, {
    method: 'PUT',
    headers: getHeaders('application/json'),
    body: JSON.stringify({ name })
  })
  if (!response.ok) throw new Error('Failed to rename playlist')
  return response.json()
}

export async function deletePlaylist(id: string): Promise<void> {
  const response = await fetch(`/api/playlists/${encodeURIComponent(id)}`, {
    method: 'DELETE',
    headers: getHeaders()
  })
  if (!response.ok) throw new Error('Failed to delete playlist')
}
