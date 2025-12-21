import type { CsvUploadResponse, MatchResponse, Song } from './types'

export async function fetchHealth(): Promise<{ status: string; timestamp: string }> {
  const response = await fetch('/api/health')
  return response.json()
}

export async function uploadCsv(file: File): Promise<CsvUploadResponse> {
  const formData = new FormData()
  formData.append('file', file)

  const response = await fetch('/api/csv/upload', {
    method: 'POST',
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
    headers: { 'Content-Type': 'application/json' },
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
