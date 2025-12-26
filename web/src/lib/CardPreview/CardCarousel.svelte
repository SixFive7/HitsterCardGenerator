<script lang="ts">
  /**
   * CardCarousel - Embla carousel with CSS 3D flip animation
   * Each card can be flipped to show front (QR code) or back (song info)
   * Uses server-rendered QuestPDF images for pixel-perfect preview
   */
  import emblaCarouselSvelte from 'embla-carousel-svelte'
  import type { EmblaOptionsType, EmblaCarouselType } from 'embla-carousel'
  import CardFront from './CardFront.svelte'
  import CardBack from './CardBack.svelte'
  import type { MatchResult } from '../types'

  interface Props {
    cards: MatchResult[]
    genreColors: Record<string, string>
    currentIndex?: number
    onIndexChange?: (index: number) => void
    flippedCards?: Set<number>
    onFlipToggle?: (index: number) => void
  }

  let { cards, genreColors, currentIndex = 0, onIndexChange, flippedCards = new Set(), onFlipToggle }: Props = $props()

  // Embla options
  const options: EmblaOptionsType = {
    loop: false,
    align: 'center'
  }

  // State
  let emblaApi = $state<EmblaCarouselType | undefined>(undefined)
  let selectedIndex = $state(0)

  // Extract track ID from Spotify URL
  function extractTrackId(spotifyUrl: string | undefined): string {
    if (!spotifyUrl) return ''
    // URL format: https://open.spotify.com/track/TRACK_ID
    const match = spotifyUrl.match(/track\/([a-zA-Z0-9]+)/)
    return match ? match[1] : ''
  }

  // Preload card images for smooth transitions
  function preloadCardImage(side: 'front' | 'back', cardData: {
    trackId: string
    title: string
    artist: string
    year: number
    genre: string
    backgroundColor: string
  }) {
    fetch(`/api/card-preview/${side}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(cardData),
      cache: 'force-cache'
    }).catch(() => {
      // Silently ignore preload errors
    })
  }

  // Preload both sides of a card
  function preloadCard(card: MatchResult, backgroundColor: string) {
    const trackId = extractTrackId(card.match?.spotifyUrl)
    if (!trackId) return

    const cardData = {
      trackId,
      title: card.originalTitle,
      artist: card.originalArtist,
      year: card.originalYear,
      genre: card.originalGenre,
      backgroundColor
    }

    preloadCardImage('front', cardData)
    preloadCardImage('back', cardData)
  }

  // Preload adjacent cards when carousel index changes
  function preloadAdjacentCards(index: number) {
    // Preload current card
    if (cards[index]) {
      preloadCard(cards[index], getGenreColor(cards[index].originalGenre))
    }

    // Preload previous card
    if (index > 0 && cards[index - 1]) {
      preloadCard(cards[index - 1], getGenreColor(cards[index - 1].originalGenre))
    }

    // Preload next card
    if (index < cards.length - 1 && cards[index + 1]) {
      preloadCard(cards[index + 1], getGenreColor(cards[index + 1].originalGenre))
    }
  }

  // Initialize embla
  function onEmblaInit(event: CustomEvent<EmblaCarouselType>) {
    emblaApi = event.detail

    // Track selected index and preload adjacent cards
    emblaApi.on('select', () => {
      selectedIndex = emblaApi.selectedScrollSnap()
      if (onIndexChange) {
        onIndexChange(selectedIndex)
      }
      // Preload adjacent cards for smooth swiping
      preloadAdjacentCards(selectedIndex)
    })

    // Initial preload
    preloadAdjacentCards(selectedIndex)
  }

  // Sync external currentIndex with embla
  $effect(() => {
    if (emblaApi && currentIndex !== selectedIndex) {
      emblaApi.scrollTo(currentIndex)
      selectedIndex = currentIndex
    }
  })

  // Toggle card flip
  function toggleFlip(index: number) {
    if (onFlipToggle) {
      onFlipToggle(index)
    }
  }

  // Check if card is flipped
  function isFlipped(index: number): boolean {
    return flippedCards.has(index)
  }

  // Get genre color with fallback
  function getGenreColor(genre: string): string {
    return genreColors[genre] || '#808080'
  }
</script>

<div class="carousel-container">
  <!-- Embla Carousel -->
  <div class="embla" use:emblaCarouselSvelte={{ options }} onemblaInit={onEmblaInit}>
    <div class="embla__container">
      {#each cards as card, index (card.index)}
        <div class="embla__slide">
          <div
            class="card-flip-container"
            class:flipped={isFlipped(index)}
            onclick={() => toggleFlip(index)}
            role="button"
            tabindex="0"
            onkeydown={(e) => {
              if (e.key === 'Enter' || e.key === ' ') {
                toggleFlip(index)
              }
            }}
            aria-label="Click to flip card"
          >
            <div class="card-flip-inner">
              <!-- Front face (QR code) -->
              <div class="card-face card-face-front">
                <CardFront
                  trackId={extractTrackId(card.match?.spotifyUrl)}
                  title={card.originalTitle}
                  artist={card.originalArtist}
                  year={card.originalYear}
                  genre={card.originalGenre}
                  backgroundColor={getGenreColor(card.originalGenre)}
                />
              </div>

              <!-- Back face (song info) -->
              <div class="card-face card-face-back">
                <CardBack
                  trackId={extractTrackId(card.match?.spotifyUrl)}
                  title={card.originalTitle}
                  artist={card.originalArtist}
                  year={card.originalYear}
                  genre={card.originalGenre}
                  backgroundColor={getGenreColor(card.originalGenre)}
                />
              </div>
            </div>
          </div>
        </div>
      {/each}
    </div>
  </div>

  <!-- Flip Hint -->
  <div class="flip-hint">
    Click card to flip
  </div>
</div>

<style>
  .carousel-container {
    width: 100%;
    max-width: 800px;
    margin: 0 auto;
  }

  /* Embla carousel styles */
  .embla {
    overflow: hidden;
    width: 100%;
  }

  .embla__container {
    display: flex;
    touch-action: pan-y;
    margin-left: calc(var(--slide-spacing, 1rem) * -1);
  }

  .embla__slide {
    flex: 0 0 100%;
    min-width: 0;
    padding-left: var(--slide-spacing, 1rem);
  }

  /* Card flip container with 3D perspective */
  .card-flip-container {
    perspective: 1000px;
    width: 100%;
    max-width: 600px;
    margin: 0 auto;
    cursor: pointer;
  }

  .card-flip-inner {
    position: relative;
    width: 100%;
    aspect-ratio: 17 / 11;
    transition: transform 0.6s cubic-bezier(0.4, 0.0, 0.2, 1);
    transform-style: preserve-3d;
  }

  .card-flip-container.flipped .card-flip-inner {
    transform: rotateY(180deg);
  }

  .card-face {
    position: absolute;
    width: 100%;
    height: 100%;
    backface-visibility: hidden;
    -webkit-backface-visibility: hidden;
  }

  .card-face-front {
    z-index: 2;
  }

  .card-face-back {
    transform: rotateY(180deg);
  }

  .flip-hint {
    text-align: center;
    margin-top: 16px;
    font-size: 14px;
    color: #666;
    font-weight: 500;
  }
</style>
