/**
 * Card customization store using Svelte 5 runes
 * Manages genre colors (read-only) and current card index
 */

// Default genre colors ported from Models/GenreColors.cs (35 genres)
// This is now the only palette - hardcoded Spotify colors
export const DEFAULT_GENRE_COLORS: Record<string, string> = {
  // Popular genres (30)
  "Rock": "#E63946",
  "Pop": "#FF69B4",
  "Hip-Hop": "#FFD700",
  "R&B": "#9B59B6",
  "Country": "#D2691E",
  "Jazz": "#6B5B95",
  "Blues": "#4169E1",
  "Electronic": "#00CED1",
  "Dance": "#FF1493",
  "House": "#32CD32",
  "Techno": "#008B8B",
  "Classical": "#1E3A5F",
  "Reggae": "#228B22",
  "Soul": "#8B0000",
  "Funk": "#FF8C00",
  "Disco": "#DA70D6",
  "Metal": "#2F4F4F",
  "Punk": "#FF00FF",
  "Alternative": "#2E8B57",
  "Indie": "#DAA520",
  "Folk": "#808000",
  "Latin": "#FF6347",
  "Rap": "#B8860B",
  "Gospel": "#FFE4B5",
  "World": "#8B4513",
  "Ambient": "#87CEEB",
  "New Wave": "#7B68EE",
  "Grunge": "#556B2F",
  "Ska": "#20B2AA",
  "Synthpop": "#FF1493",

  // French genres (5)
  "Chanson": "#0055A4",
  "Variete Francaise": "#3B5998",
  "French Pop": "#FF69B4",
  "French Hip-Hop": "#FFD700",
  "Musette": "#EF4135"
}

// Create the store state for carousel
let currentCardIndex = $state<number>(0)

/**
 * Gets the color for a genre (with fallback to gray)
 * Now reads directly from DEFAULT_GENRE_COLORS (no customization)
 */
export function getGenreColor(genre: string): string {
  return DEFAULT_GENRE_COLORS[genre] || '#808080'
}

// Export reactive state for carousel
export function getCardCustomizationState() {
  return {
    get currentCardIndex() { return currentCardIndex },
    set currentCardIndex(value: number) { currentCardIndex = value }
  }
}
