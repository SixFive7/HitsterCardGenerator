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
