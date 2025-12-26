<script lang="ts">
  import type { PlaylistTrack } from './types'
  import { DEFAULT_GENRE_COLORS } from './stores/cardCustomization.svelte'

  // Props
  interface Props {
    tracks: PlaylistTrack[]
    onRemoveTrack: (trackId: string) => void | Promise<void>
    onUpdateGenre: (trackId: string, genre: string) => void
    onContinueToPreview: () => void
  }

  let { tracks, onRemoveTrack, onUpdateGenre, onContinueToPreview }: Props = $props()

  // Get genre options from DEFAULT_GENRE_COLORS
  const genreOptions = Object.keys(DEFAULT_GENRE_COLORS).sort()

  function handleRemove(trackId: string) {
    onRemoveTrack(trackId)
  }

  function handleGenreChange(trackId: string, event: Event) {
    const target = event.target as HTMLSelectElement
    onUpdateGenre(trackId, target.value)
  }
</script>

<div class="playlist-builder">
  <!-- Header -->
  <div class="header">
    <h2 class="title">Your Playlist</h2>
    {#if tracks.length > 0}
      <span class="count-badge">{tracks.length}</span>
    {/if}
  </div>

  <!-- Track List -->
  <div class="track-list">
    {#if tracks.length === 0}
      <div class="empty-state">
        <div class="empty-icon">🎵</div>
        <p>Search for tracks and add them to your playlist</p>
      </div>
    {:else}
      {#each tracks as track (track.trackId)}
        <div class="track-row">
          {#if track.albumImageUrl}
            <img
              src={track.albumImageUrl}
              alt="{track.albumName} album art"
              class="album-art"
            />
          {:else}
            <div class="album-art-placeholder">🎵</div>
          {/if}

          <div class="track-info">
            <div class="track-name">{track.trackName}</div>
            <div class="track-details">
              <span class="artist-name">{track.artistName}</span>
              {#if track.releaseYear > 0}
                <span class="year">• {track.releaseYear}</span>
              {/if}
            </div>
          </div>

          <select
            value={track.genre}
            onchange={(e) => handleGenreChange(track.trackId, e)}
            class="genre-select"
          >
            {#each genreOptions as genre}
              <option value={genre}>{genre}</option>
            {/each}
          </select>

          <button
            onclick={() => handleRemove(track.trackId)}
            class="remove-button"
            title="Remove from playlist"
          >
            ✕
          </button>
        </div>
      {/each}
    {/if}
  </div>

  <!-- Footer -->
  {#if tracks.length > 0}
    <div class="footer">
      <button
        onclick={onContinueToPreview}
        class="continue-button"
      >
        Continue to Preview
      </button>
    </div>
  {/if}
</div>

<style>
  .playlist-builder {
    width: 100%;
    max-width: 800px;
    margin: 0 auto;
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .header {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    padding: 0 0.5rem;
  }

  .title {
    margin: 0;
    font-size: 1.5rem;
    font-weight: 700;
    color: white;
  }

  .count-badge {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    min-width: 2rem;
    height: 2rem;
    padding: 0 0.5rem;
    background-color: #1DB954;
    color: white;
    border-radius: 1rem;
    font-size: 0.875rem;
    font-weight: 600;
  }

  .track-list {
    background-color: #282828;
    border-radius: 0.75rem;
    padding: 0.5rem;
    max-height: 500px;
    overflow-y: auto;
  }

  .track-list::-webkit-scrollbar {
    width: 8px;
  }

  .track-list::-webkit-scrollbar-track {
    background: #191414;
    border-radius: 0.5rem;
  }

  .track-list::-webkit-scrollbar-thumb {
    background: #383838;
    border-radius: 0.5rem;
  }

  .track-list::-webkit-scrollbar-thumb:hover {
    background: #484848;
  }

  .empty-state {
    text-align: center;
    padding: 3rem 1rem;
    color: #888;
  }

  .empty-icon {
    font-size: 4rem;
    margin-bottom: 1rem;
  }

  .empty-state p {
    margin: 0;
    font-size: 1rem;
  }

  .track-row {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 0.75rem;
    border-radius: 0.5rem;
    transition: background-color 0.2s;
  }

  .track-row:hover {
    background-color: #383838;
  }

  .album-art {
    width: 48px;
    height: 48px;
    border-radius: 0.25rem;
    object-fit: cover;
    flex-shrink: 0;
  }

  .album-art-placeholder {
    width: 48px;
    height: 48px;
    border-radius: 0.25rem;
    background-color: #383838;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.5rem;
    flex-shrink: 0;
  }

  .track-info {
    flex: 1;
    min-width: 0;
  }

  .track-name {
    color: white;
    font-weight: 600;
    font-size: 0.95rem;
    margin-bottom: 0.25rem;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .track-details {
    color: #b3b3b3;
    font-size: 0.8rem;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .artist-name {
    color: #b3b3b3;
  }

  .year {
    color: #888;
  }

  .genre-select {
    padding: 0.5rem 0.75rem;
    background-color: #383838;
    border: 1px solid #484848;
    border-radius: 0.5rem;
    color: white;
    font-size: 0.875rem;
    cursor: pointer;
    transition: all 0.2s;
    flex-shrink: 0;
    min-width: 140px;
  }

  .genre-select:hover {
    background-color: #484848;
    border-color: #585858;
  }

  .genre-select:focus {
    outline: none;
    border-color: #1DB954;
    box-shadow: 0 0 0 2px rgba(29, 185, 84, 0.1);
  }

  .genre-select option {
    background-color: #282828;
    color: white;
  }

  .remove-button {
    width: 2rem;
    height: 2rem;
    border-radius: 9999px;
    background-color: #FF6B6B;
    color: white;
    border: none;
    font-size: 1.25rem;
    font-weight: bold;
    cursor: pointer;
    transition: all 0.2s;
    flex-shrink: 0;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .remove-button:hover {
    background-color: #ff5252;
    transform: scale(1.05);
  }

  .remove-button:active {
    transform: scale(0.95);
  }

  .footer {
    padding: 0 0.5rem;
  }

  .continue-button {
    width: 100%;
    padding: 1rem;
    background-color: #1DB954;
    color: white;
    border: none;
    border-radius: 0.75rem;
    font-size: 1rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
  }

  .continue-button:hover {
    background-color: #1ed760;
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(29, 185, 84, 0.3);
  }

  .continue-button:active {
    transform: translateY(0);
  }

  .continue-button:disabled {
    background-color: #383838;
    color: #888;
    cursor: not-allowed;
    transform: none;
  }
</style>
