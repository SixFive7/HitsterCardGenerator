/**
 * Card customization store using Svelte 5 runes
 * Manages genre colors, card inclusion, and current card index
 */

// Default genre colors ported from Models/GenreColors.cs (35 genres)
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

// LocalStorage keys
const STORAGE_KEY_COLORS = 'hitster-genre-colors'
const STORAGE_KEY_INCLUDED = 'hitster-included-cards'

// Load from localStorage
function loadGenreColors(): Record<string, string> {
  if (typeof window === 'undefined') return { ...DEFAULT_GENRE_COLORS }
  try {
    const stored = localStorage.getItem(STORAGE_KEY_COLORS)
    if (stored) {
      return JSON.parse(stored)
    }
  } catch (e) {
    console.error('Failed to load genre colors from localStorage', e)
  }
  return { ...DEFAULT_GENRE_COLORS }
}

function loadIncludedCards(): Set<number> {
  if (typeof window === 'undefined') return new Set()
  try {
    const stored = localStorage.getItem(STORAGE_KEY_INCLUDED)
    if (stored) {
      const arr = JSON.parse(stored) as number[]
      return new Set(arr)
    }
  } catch (e) {
    console.error('Failed to load included cards from localStorage', e)
  }
  return new Set()
}

// Save to localStorage
function saveGenreColors(colors: Record<string, string>) {
  if (typeof window === 'undefined') return
  try {
    localStorage.setItem(STORAGE_KEY_COLORS, JSON.stringify(colors))
  } catch (e) {
    console.error('Failed to save genre colors to localStorage', e)
  }
}

function saveIncludedCards(cards: Set<number>) {
  if (typeof window === 'undefined') return
  try {
    localStorage.setItem(STORAGE_KEY_INCLUDED, JSON.stringify(Array.from(cards)))
  } catch (e) {
    console.error('Failed to save included cards to localStorage', e)
  }
}

// Create the store state
let genreColors = $state<Record<string, string>>(loadGenreColors())
let includedCards = $state<Set<number>>(loadIncludedCards())
let currentCardIndex = $state<number>(0)

/**
 * Sets a custom color for a genre
 */
export function setGenreColor(genre: string, color: string) {
  genreColors[genre] = color
  saveGenreColors(genreColors)
}

/**
 * Gets the color for a genre (with fallback to gray)
 */
export function getGenreColor(genre: string): string {
  return genreColors[genre] || '#808080'
}

/**
 * Toggles whether a card is included in the final export
 */
export function toggleCardInclusion(cardIndex: number) {
  if (includedCards.has(cardIndex)) {
    includedCards.delete(cardIndex)
  } else {
    includedCards.add(cardIndex)
  }
  // Trigger reactivity
  includedCards = new Set(includedCards)
  saveIncludedCards(includedCards)
}

/**
 * Resets all genre colors to defaults
 */
export function resetGenreColors() {
  genreColors = { ...DEFAULT_GENRE_COLORS }
  saveGenreColors(genreColors)
}

/**
 * Applies a palette to all genres
 */
export function applyPalette(palette: Record<string, string>) {
  genreColors = { ...palette }
  saveGenreColors(genreColors)
}

/**
 * Initializes included cards (call with total count)
 */
export function initializeIncludedCards(totalCards: number) {
  includedCards = new Set(Array.from({ length: totalCards }, (_, i) => i))
  saveIncludedCards(includedCards)
}

/**
 * Checks if a card is included
 */
export function isCardIncluded(cardIndex: number): boolean {
  return includedCards.has(cardIndex)
}

/**
 * Gets the count of included cards
 */
export function getIncludedCount(): number {
  return includedCards.size
}

// Export reactive state
export function getCardCustomizationState() {
  return {
    get genreColors() { return genreColors },
    get includedCards() { return includedCards },
    get currentCardIndex() { return currentCardIndex },
    set currentCardIndex(value: number) { currentCardIndex = value }
  }
}
