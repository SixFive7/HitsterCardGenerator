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
