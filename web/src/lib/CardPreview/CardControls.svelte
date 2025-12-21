<script lang="ts">
  /**
   * CardControls - Navigation and card curation controls
   * Provides prev/next navigation, card counter, and include/exclude toggle
   */

  interface Props {
    totalCards: number
    currentIndex: number
    isIncluded: boolean
    onPrev: () => void
    onNext: () => void
    onToggleInclude: () => void
    onFlip: () => void
  }

  let { totalCards, currentIndex, isIncluded, onPrev, onNext, onToggleInclude, onFlip }: Props = $props()

  // Computed values
  const isFirstCard = $derived(currentIndex === 0)
  const isLastCard = $derived(currentIndex === totalCards - 1)
</script>

<div class="card-controls">
  <!-- Left: Previous button -->
  <div class="controls-section controls-left">
    <button
      class="nav-button prev-button"
      onclick={onPrev}
      disabled={isFirstCard}
      aria-label="Previous card"
    >
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polyline points="15 18 9 12 15 6"></polyline>
      </svg>
      <span>Previous</span>
    </button>
  </div>

  <!-- Center: Counter and Include/Exclude toggle -->
  <div class="controls-section controls-center">
    <div class="card-counter">
      Card <span class="current">{currentIndex + 1}</span> of <span class="total">{totalCards}</span>
    </div>

    <button
      class="toggle-button"
      class:included={isIncluded}
      class:excluded={!isIncluded}
      onclick={onToggleInclude}
      aria-label={isIncluded ? 'Exclude card' : 'Include card'}
    >
      {#if isIncluded}
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <polyline points="20 6 9 17 4 12"></polyline>
        </svg>
        <span>Included</span>
      {:else}
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <line x1="18" y1="6" x2="6" y2="18"></line>
          <line x1="6" y1="6" x2="18" y2="18"></line>
        </svg>
        <span>Excluded</span>
      {/if}
    </button>

    <button
      class="flip-button"
      onclick={onFlip}
      aria-label="Flip card"
    >
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polyline points="23 4 23 10 17 10"></polyline>
        <polyline points="1 20 1 14 7 14"></polyline>
        <path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15"></path>
      </svg>
      <span>Flip</span>
    </button>
  </div>

  <!-- Right: Next button -->
  <div class="controls-section controls-right">
    <button
      class="nav-button next-button"
      onclick={onNext}
      disabled={isLastCard}
      aria-label="Next card"
    >
      <span>Next</span>
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <polyline points="9 18 15 12 9 6"></polyline>
      </svg>
    </button>
  </div>
</div>

<style>
  .card-controls {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 16px;
    padding: 20px;
    background: #282828;
    border-radius: 12px;
    margin-top: 24px;
  }

  .controls-section {
    display: flex;
    align-items: center;
    gap: 12px;
  }

  .controls-left,
  .controls-right {
    flex: 0 0 auto;
  }

  .controls-center {
    flex: 1;
    justify-content: center;
  }

  /* Navigation buttons */
  .nav-button {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 10px 20px;
    background: #1a1a1a;
    border: 2px solid #333;
    border-radius: 8px;
    color: white;
    font-weight: 600;
    font-size: 14px;
    cursor: pointer;
    transition: all 0.2s ease;
  }

  .nav-button:hover:not(:disabled) {
    border-color: #1DB954;
    background: #1DB954;
    transform: translateY(-2px);
  }

  .nav-button:disabled {
    opacity: 0.3;
    cursor: not-allowed;
  }

  /* Card counter */
  .card-counter {
    font-size: 16px;
    font-weight: 600;
    color: white;
    padding: 8px 16px;
    background: #1a1a1a;
    border-radius: 8px;
  }

  .card-counter .current {
    color: #1DB954;
  }

  .card-counter .total {
    color: #999;
  }

  /* Toggle button */
  .toggle-button {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 10px 20px;
    border: 2px solid;
    border-radius: 8px;
    font-weight: 600;
    font-size: 14px;
    cursor: pointer;
    transition: all 0.2s ease;
  }

  .toggle-button.included {
    background: #1DB954;
    border-color: #1DB954;
    color: white;
  }

  .toggle-button.included:hover {
    background: #1ed760;
    border-color: #1ed760;
    transform: scale(1.05);
  }

  .toggle-button.excluded {
    background: #FF6B6B;
    border-color: #FF6B6B;
    color: white;
  }

  .toggle-button.excluded:hover {
    background: #ff8888;
    border-color: #ff8888;
    transform: scale(1.05);
  }

  /* Flip button */
  .flip-button {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 10px 20px;
    background: #1a1a1a;
    border: 2px solid #333;
    border-radius: 8px;
    color: white;
    font-weight: 600;
    font-size: 14px;
    cursor: pointer;
    transition: all 0.2s ease;
  }

  .flip-button:hover {
    border-color: #1DB954;
    background: #1DB954;
    transform: rotate(180deg);
  }

  /* Responsive layout */
  @media (max-width: 768px) {
    .card-controls {
      flex-direction: column;
      gap: 12px;
    }

    .controls-section {
      width: 100%;
      justify-content: center;
    }

    .nav-button span,
    .toggle-button span,
    .flip-button span {
      display: none;
    }

    .nav-button,
    .toggle-button,
    .flip-button {
      padding: 12px;
      justify-content: center;
    }
  }
</style>
