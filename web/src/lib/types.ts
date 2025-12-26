export interface Song {
  title: string
  artist: string
  year: number
  genre: string
  validationErrors: string[]
}

export interface CsvUploadResponse {
  success: boolean
  totalSongs: number
  validSongs: Song[]
  invalidSongs: Song[]
  errorSummary: string
}

export interface SpotifyMatch {
  trackId: string
  trackName: string
  artistName: string
  albumName: string
  albumImageUrl: string
  spotifyUrl: string
}

export interface SearchResult {
  trackId: string
  trackName: string
  artistName: string
  albumName: string
  albumImageUrl: string
  spotifyUrl: string
  releaseYear: number
}

export interface PlaylistTrack {
  trackId: string
  trackName: string
  artistName: string
  albumName: string
  albumImageUrl: string
  spotifyUrl: string
  releaseYear: number
  genre: string
}

export interface MatchResult {
  index: number
  originalTitle: string
  originalArtist: string
  originalYear: number
  originalGenre: string
  match: SpotifyMatch | null
  confidence: 'high' | 'medium' | 'low' | 'none'
  alternatives: SpotifyMatch[]
}

export interface MatchResponse {
  success: boolean
  results: MatchResult[]
  totalMatched: number
  totalNotFound: number
}

export interface CardCustomization {
  genreColors: Record<string, string>
  currentCardIndex: number
}

export type CuttingLineStyle = 'None' | 'EdgeOnly' | 'Complete'

export interface ExportRequest {
  cards: ExportCard[]
  genreColors: Record<string, string>
  cuttingLines: CuttingLineStyle
}

export interface ExportCard {
  trackId: string
  title: string
  artist: string
  year: number
  genre: string
  albumImageUrl?: string
  albumName?: string
}

export interface Playlist {
  id: string
  browserId: string
  name: string
  trackCount: number
  createdAt: string
  updatedAt: string
}
