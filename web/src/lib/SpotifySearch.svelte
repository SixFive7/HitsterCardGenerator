<script lang="ts">
  import { searchSpotify } from './api'
  import type { SearchResult } from './types'
  import { onMount } from 'svelte'

  // State using Svelte 5 runes
  let query = $state('')
  let results = $state<SearchResult[]>([])
  let isLoading = $state(false)
  let searchInput: HTMLInputElement

  // Debounced search
  let debounceTimer: ReturnType<typeof setTimeout> | null = null

  async function performSearch() {
    isLoading = true
    try {
      results = await searchSpotify(query)
    } catch (error) {
      console.error('Search error:', error)
      results = []
    } finally {
      isLoading = false
    }
  }

  function handleInput() {
    // Clear existing timer
    if (debounceTimer) {
      clearTimeout(debounceTimer)
    }

    // Set new timer for 300ms debounce
    debounceTimer = setTimeout(() => {
      performSearch()
    }, 300)
  }

  // Auto-focus on mount
  onMount(() => {
    searchInput?.focus()
  })
</script>

<div class="spotify-search">
  <!-- Search Input -->
  <div class="search-input-container">
    <input
      bind:this={searchInput}
      bind:value={query}
      oninput={handleInput}
      type="text"
      placeholder="Search Spotify..."
      class="search-input"
    />
    {#if isLoading}
      <div class="loading-spinner"></div>
    {/if}
  </div>

  <!-- Results -->
  <div class="results-container">
    {#if results.length > 0}
      {#each results as result}
        <div class="result-card">
          {#if result.albumImageUrl}
            <img
              src={result.albumImageUrl}
              alt="{result.albumName} album art"
              class="album-art"
            />
          {:else}
            <div class="album-art-placeholder">🎵</div>
          {/if}
          <div class="result-info">
            <div class="track-name">{result.trackName}</div>
            <div class="track-details">
              <span class="artist-name">{result.artistName}</span>
              {#if result.releaseYear > 0}
                <span class="year">• {result.releaseYear}</span>
              {/if}
            </div>
          </div>
        </div>
      {/each}
    {:else if query.trim() && !isLoading}
      <div class="no-results">
        <div class="no-results-icon">🔍</div>
        <p>No results found for "{query}"</p>
      </div>
    {:else if !query.trim() && !isLoading}
      <div class="no-results">
        <div class="no-results-icon">🎵</div>
        <p>Search for tracks on Spotify</p>
      </div>
    {/if}
  </div>
</div>

<style>
  .spotify-search {
    width: 100%;
    max-width: 600px;
    margin: 0 auto;
  }

  .search-input-container {
    position: relative;
    margin-bottom: 1rem;
  }

  .search-input {
    width: 100%;
    padding: 1rem 3rem 1rem 1rem;
    font-size: 1rem;
    background-color: #282828;
    border: 2px solid #383838;
    border-radius: 0.75rem;
    color: white;
    transition: all 0.2s;
  }

  .search-input:focus {
    outline: none;
    border-color: #1DB954;
    box-shadow: 0 0 0 3px rgba(29, 185, 84, 0.1);
  }

  .search-input::placeholder {
    color: #888;
  }

  .loading-spinner {
    position: absolute;
    right: 1rem;
    top: 50%;
    transform: translateY(-50%);
    width: 1.5rem;
    height: 1.5rem;
    border: 3px solid #383838;
    border-top-color: #1DB954;
    border-radius: 50%;
    animation: spin 0.6s linear infinite;
  }

  @keyframes spin {
    to { transform: translateY(-50%) rotate(360deg); }
  }

  .results-container {
    max-height: 500px;
    overflow-y: auto;
    background-color: #282828;
    border-radius: 0.75rem;
    padding: 0.5rem;
  }

  .results-container::-webkit-scrollbar {
    width: 8px;
  }

  .results-container::-webkit-scrollbar-track {
    background: #191414;
    border-radius: 0.5rem;
  }

  .results-container::-webkit-scrollbar-thumb {
    background: #383838;
    border-radius: 0.5rem;
  }

  .results-container::-webkit-scrollbar-thumb:hover {
    background: #484848;
  }

  .result-card {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 0.75rem;
    border-radius: 0.5rem;
    transition: background-color 0.2s;
    cursor: pointer;
  }

  .result-card:hover {
    background-color: #383838;
  }

  .album-art {
    width: 64px;
    height: 64px;
    border-radius: 0.25rem;
    object-fit: cover;
    flex-shrink: 0;
  }

  .album-art-placeholder {
    width: 64px;
    height: 64px;
    border-radius: 0.25rem;
    background-color: #383838;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 2rem;
    flex-shrink: 0;
  }

  .result-info {
    flex: 1;
    min-width: 0;
  }

  .track-name {
    color: white;
    font-weight: 600;
    font-size: 1rem;
    margin-bottom: 0.25rem;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .track-details {
    color: #b3b3b3;
    font-size: 0.875rem;
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

  .no-results {
    text-align: center;
    padding: 3rem 1rem;
    color: #888;
  }

  .no-results-icon {
    font-size: 4rem;
    margin-bottom: 1rem;
  }

  .no-results p {
    margin: 0;
    font-size: 1rem;
  }
</style>
