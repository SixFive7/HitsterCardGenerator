<script lang="ts">
  /**
   * CardCarousel - Embla carousel with CSS 3D flip animation
   * Each card can be flipped to show front (album art) or back (song info)
   */
  import emblaCarouselSvelte from 'embla-carousel-svelte'
  import type { EmblaOptionsType, EmblaCarouselType } from 'embla-carousel'
  import CardFront from './CardFront.svelte'
  import CardBack from './CardBack.svelte'
  import type { MatchResult } from '../types'

  interface Props {
    cards: MatchResult[]
    genreColors: Record<string, string>
  }

  let { cards, genreColors }: Props = $props()

  // Embla options
  const options: EmblaOptionsType = {
    loop: false,
    align: 'center'
  }

  // State
  let emblaApi = $state<EmblaCarouselType | undefined>(undefined)
  let selectedIndex = $state(0)
  let flippedCards = $state<Set<number>>(new Set())

  // Initialize embla
  function onEmblaInit(event: CustomEvent<EmblaCarouselType>) {
    emblaApi = event.detail

    // Track selected index
    emblaApi.on('select', () => {
      selectedIndex = emblaApi.selectedScrollSnap()
    })
  }

  // Toggle card flip
  function toggleFlip(index: number) {
    if (flippedCards.has(index)) {
      flippedCards.delete(index)
    } else {
      flippedCards.add(index)
    }
    flippedCards = new Set(flippedCards)
  }

  // Check if card is flipped
  function isFlipped(index: number): boolean {
    return flippedCards.has(index)
  }

  // Get genre color with fallback
  function getGenreColor(genre: string): string {
    return genreColors[genre] || '#808080'
  }

  // Navigation handlers
  function scrollPrev() {
    emblaApi?.scrollPrev()
  }

  function scrollNext() {
    emblaApi?.scrollNext()
  }

  // Computed values
  const canScrollPrev = $derived(emblaApi?.canScrollPrev() ?? false)
  const canScrollNext = $derived(emblaApi?.canScrollNext() ?? false)
</script>

<div class="carousel-container">
  <!-- Navigation Header -->
  <div class="carousel-header">
    <button
      class="nav-button"
      onclick={scrollPrev}
      disabled={!canScrollPrev}
      aria-label="Previous card"
    >
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polyline points="15 18 9 12 15 6"></polyline>
      </svg>
    </button>

    <div class="carousel-counter">
      <span class="current">{selectedIndex + 1}</span>
      <span class="separator">/</span>
      <span class="total">{cards.length}</span>
    </div>

    <button
      class="nav-button"
      onclick={scrollNext}
      disabled={!canScrollNext}
      aria-label="Next card"
    >
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polyline points="9 18 15 12 9 6"></polyline>
      </svg>
    </button>
  </div>

  <!-- Embla Carousel -->
  <div class="embla" use:emblaCarouselSvelte={{ options }} on:emblaInit={onEmblaInit}>
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
              <!-- Front face (album art) -->
              <div class="card-face card-face-front">
                <CardFront
                  spotifyUrl={card.match?.spotifyUrl || ''}
                  albumImageUrl={card.match?.albumImageUrl || null}
                />
              </div>

              <!-- Back face (song info) -->
              <div class="card-face card-face-back">
                <CardBack
                  artist={card.originalArtist}
                  year={card.originalYear}
                  title={card.originalTitle}
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

  .carousel-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 24px;
    padding: 0 16px;
  }

  .nav-button {
    background: #282828;
    border: none;
    color: white;
    width: 48px;
    height: 48px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all 0.2s ease;
  }

  .nav-button:hover:not(:disabled) {
    background: #1DB954;
    transform: scale(1.1);
  }

  .nav-button:disabled {
    opacity: 0.3;
    cursor: not-allowed;
  }

  .carousel-counter {
    font-size: 24px;
    font-weight: 600;
    color: white;
  }

  .carousel-counter .current {
    color: #1DB954;
  }

  .carousel-counter .separator {
    color: #666;
    margin: 0 8px;
  }

  .carousel-counter .total {
    color: #999;
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
