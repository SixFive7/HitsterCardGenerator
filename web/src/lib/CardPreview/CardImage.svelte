<script lang="ts">
  /**
   * CardImage - Server-rendered card image component
   * Fetches QuestPDF-generated card images from the server for pixel-perfect preview
   */
  import { onDestroy } from 'svelte'

  interface Props {
    side: 'front' | 'back'
    trackId: string
    title: string
    artist: string
    year: number
    genre: string
    backgroundColor: string
  }

  let { side, trackId, title, artist, year, genre, backgroundColor }: Props = $props()

  // State for image loading
  let imageUrl = $state<string | null>(null)
  let loading = $state(true)
  let error = $state<string | null>(null)

  // Track object URLs for cleanup
  let currentObjectUrl: string | null = null

  // Fetch image when props change
  $effect(() => {
    // Capture current values for the async operation
    const currentSide = side
    const currentTrackId = trackId
    const currentTitle = title
    const currentArtist = artist
    const currentYear = year
    const currentGenre = genre
    const currentBgColor = backgroundColor

    // Reset state
    loading = true
    error = null

    // Clean up previous object URL
    if (currentObjectUrl) {
      URL.revokeObjectURL(currentObjectUrl)
      currentObjectUrl = null
      imageUrl = null
    }

    // Fetch the card image from server
    fetchCardImage(currentSide, currentTrackId, currentTitle, currentArtist, currentYear, currentGenre, currentBgColor)
  })

  async function fetchCardImage(
    side: 'front' | 'back',
    trackId: string,
    title: string,
    artist: string,
    year: number,
    genre: string,
    backgroundColor: string
  ) {
    try {
      const response = await fetch(`/api/card-preview/${side}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          trackId,
          title,
          artist,
          year,
          genre,
          backgroundColor
        })
      })

      if (!response.ok) {
        throw new Error(`Failed to load card image: ${response.status}`)
      }

      const blob = await response.blob()
      const url = URL.createObjectURL(blob)

      // Store for cleanup and set as image source
      currentObjectUrl = url
      imageUrl = url
      loading = false
    } catch (e) {
      error = e instanceof Error ? e.message : 'Failed to load card'
      loading = false
    }
  }

  // Cleanup object URL on component destroy
  onDestroy(() => {
    if (currentObjectUrl) {
      URL.revokeObjectURL(currentObjectUrl)
    }
  })
</script>

<div class="card-image-container">
  {#if loading}
    <div class="loading-state">
      <div class="spinner"></div>
    </div>
  {:else if error}
    <div class="error-state">
      <div class="error-icon">!</div>
      <div class="error-text">Failed to load card</div>
    </div>
  {:else if imageUrl}
    <img
      src={imageUrl}
      alt="Card {side}"
      class="card-image"
    />
  {/if}
</div>

<style>
  .card-image-container {
    width: 100%;
    height: 100%;
    aspect-ratio: 17 / 11;
    border-radius: 12px;
    overflow: hidden;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    background: #f0f0f0;
    display: flex;
    align-items: center;
    justify-content: center;
    backface-visibility: hidden;
    -webkit-backface-visibility: hidden;
  }

  .card-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
    display: block;
  }

  .loading-state {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 100%;
  }

  .spinner {
    width: 40px;
    height: 40px;
    border: 3px solid #e0e0e0;
    border-top-color: #666;
    border-radius: 50%;
    animation: spin 0.8s linear infinite;
  }

  @keyframes spin {
    to {
      transform: rotate(360deg);
    }
  }

  .error-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 8px;
    color: #666;
  }

  .error-icon {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    background: #e0e0e0;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: bold;
    color: #999;
  }

  .error-text {
    font-size: 12px;
    font-weight: 500;
  }
</style>
