<script lang="ts">
  /**
   * CardControls - Navigation and card curation controls
   * Provides prev/next navigation, card counter, and flip button
   */

  interface Props {
    totalCards: number
    currentIndex: number
    onPrev: () => void
    onNext: () => void
    onFlip: () => void
  }

  let { totalCards, currentIndex, onPrev, onNext, onFlip }: Props = $props()

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

  <!-- Center: Counter and Flip button -->
  <div class="controls-section controls-center">
    <div class="card-counter">
      Card <span class="current">{currentIndex + 1}</span> of <span class="total">{totalCards}</span>
    </div>

    <button
      class="flip-button"
      onclick={() => onFlip()}
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
    .flip-button span {
      display: none;
    }

    .nav-button,
    .flip-button {
      padding: 12px;
      justify-content: center;
    }
  }
</style>
