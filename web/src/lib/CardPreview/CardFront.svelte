<script lang="ts">
  /**
   * CardFront - Front face of the Hitster card
   * Displays album art as placeholder for QR code (actual QR generation is Phase 9)
   */
  interface Props {
    spotifyUrl: string
    albumImageUrl: string | null
  }

  let { spotifyUrl, albumImageUrl }: Props = $props()
</script>

<div class="card-front">
  <div class="card-content">
    {#if albumImageUrl}
      <img
        src={albumImageUrl}
        alt="Album artwork"
        class="album-art"
      />
    {:else}
      <div class="placeholder">
        <div class="placeholder-icon">🎵</div>
        <div class="placeholder-text">No Album Art</div>
      </div>
    {/if}

    <div class="qr-label">
      Scan to listen on Spotify
    </div>
  </div>
</div>

<style>
  .card-front {
    width: 100%;
    height: 100%;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    aspect-ratio: 17 / 11;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 24px;
    backface-visibility: hidden;
  }

  .card-content {
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 16px;
  }

  .album-art {
    width: 100%;
    max-width: 280px;
    height: auto;
    aspect-ratio: 1 / 1;
    border-radius: 8px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
    object-fit: cover;
  }

  .placeholder {
    width: 100%;
    max-width: 280px;
    aspect-ratio: 1 / 1;
    background: linear-gradient(135deg, #f0f0f0 0%, #e0e0e0 100%);
    border-radius: 8px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 8px;
  }

  .placeholder-icon {
    font-size: 64px;
    opacity: 0.3;
  }

  .placeholder-text {
    font-size: 14px;
    color: #666;
    font-weight: 500;
  }

  .qr-label {
    font-size: 12px;
    color: #666;
    text-align: center;
    font-weight: 500;
  }
</style>
