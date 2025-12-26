<script lang="ts">
  /**
   * GenreColorPicker - Read-only genre color reference view
   * Shows all genres with their assigned Spotify palette colors
   */
  import { getGenreColor } from '../stores/cardCustomization.svelte'

  interface Props {
    genres: string[]
  }

  let { genres }: Props = $props()
</script>

<div class="genre-color-picker">
  <!-- Genre List (read-only) -->
  <div class="genre-section">
    <h3 class="section-title">Genre Colors</h3>
    <div class="genre-list">
      {#each genres as genre}
        <div class="genre-item">
          <div class="genre-color-swatch" style="background-color: {getGenreColor(genre)};"></div>
          <span class="genre-name">{genre}</span>
        </div>
      {/each}
    </div>
  </div>
</div>

<style>
  .genre-color-picker {
    background: #282828;
    border-radius: 16px;
    padding: 20px;
    color: white;
  }

  .section-title {
    font-size: 18px;
    font-weight: 600;
    color: white;
    margin-bottom: 12px;
  }

  .genre-section {
    margin-top: 0;
  }

  .genre-list {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
    gap: 8px;
    max-height: 400px;
    overflow-y: auto;
    padding: 8px;
    background: #1a1a1a;
    border-radius: 8px;
  }

  .genre-item {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 10px 12px;
    background: #282828;
    border: 2px solid #333;
    border-radius: 8px;
    text-align: left;
  }

  .genre-color-swatch {
    width: 24px;
    height: 24px;
    border-radius: 4px;
    border: 2px solid rgba(255, 255, 255, 0.3);
    flex-shrink: 0;
  }

  .genre-name {
    font-size: 14px;
    font-weight: 500;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  /* Custom scrollbar for genre list */
  .genre-list::-webkit-scrollbar {
    width: 8px;
  }

  .genre-list::-webkit-scrollbar-track {
    background: #1a1a1a;
    border-radius: 4px;
  }

  .genre-list::-webkit-scrollbar-thumb {
    background: #1DB954;
    border-radius: 4px;
  }

  .genre-list::-webkit-scrollbar-thumb:hover {
    background: #1ed760;
  }
</style>
