<script lang="ts">
  import type { Playlist } from '../types'
  import PlaylistCard from './PlaylistCard.svelte'

  interface Props {
    playlists: Playlist[]
    selectedId: string | null
    onSelect: (id: string) => void
    onCreateNew: () => void
    onEdit?: (playlist: Playlist) => void
    onDelete?: (playlist: Playlist) => void
  }

  let { playlists, selectedId, onSelect, onCreateNew, onEdit, onDelete }: Props = $props()
</script>

<div class="playlist-list">
  {#if playlists.length === 0}
    <div class="empty-state">
      <div class="empty-icon">
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="w-16 h-16">
          <path fill-rule="evenodd" d="M19.952 1.651a.75.75 0 01.298.599V16.303a3 3 0 01-2.176 2.884l-1.32.377a2.553 2.553 0 11-1.403-4.909l2.311-.66a1.5 1.5 0 001.088-1.442V6.994l-9 2.572v9.737a3 3 0 01-2.176 2.884l-1.32.377a2.553 2.553 0 11-1.402-4.909l2.31-.66a1.5 1.5 0 001.088-1.442V5.25a.75.75 0 01.544-.721l10.5-3a.75.75 0 01.456.122z" clip-rule="evenodd" />
        </svg>
      </div>
      <p>No playlists yet</p>
      <button onclick={onCreateNew} class="create-first-button">
        Create your first playlist
      </button>
    </div>
  {:else}
    <div class="grid">
      {#each playlists as playlist (playlist.id)}
        <PlaylistCard
          {playlist}
          isSelected={selectedId === playlist.id}
          {onSelect}
          {onEdit}
          {onDelete}
        />
      {/each}

      <!-- Create New Card -->
      <button onclick={onCreateNew} class="create-card">
        <div class="create-icon">
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="w-8 h-8">
            <path fill-rule="evenodd" d="M12 3.75a.75.75 0 01.75.75v6.75h6.75a.75.75 0 010 1.5h-6.75v6.75a.75.75 0 01-1.5 0v-6.75H4.5a.75.75 0 010-1.5h6.75V4.5a.75.75 0 01.75-.75z" clip-rule="evenodd" />
          </svg>
        </div>
        <span class="create-text">Create New Playlist</span>
      </button>
    </div>
  {/if}
</div>

<style>
  .playlist-list {
    width: 100%;
  }

  .empty-state {
    text-align: center;
    padding: 3rem 1rem;
    color: #888;
  }

  .empty-icon {
    margin-bottom: 1rem;
    color: #555;
    display: flex;
    justify-content: center;
  }

  .empty-state p {
    margin: 0 0 1.5rem 0;
    font-size: 1.125rem;
  }

  .create-first-button {
    padding: 0.875rem 1.5rem;
    background-color: #1DB954;
    color: white;
    border: none;
    border-radius: 0.5rem;
    font-size: 1rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
  }

  .create-first-button:hover {
    background-color: #1ed760;
    transform: scale(1.02);
  }

  .grid {
    display: grid;
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  @media (min-width: 640px) {
    .grid {
      grid-template-columns: repeat(2, 1fr);
    }
  }

  @media (min-width: 1024px) {
    .grid {
      grid-template-columns: repeat(3, 1fr);
    }
  }

  .create-card {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 0.75rem;
    padding: 2rem;
    background-color: #282828;
    border: 2px dashed #484848;
    border-radius: 0.75rem;
    cursor: pointer;
    transition: all 0.2s ease;
    min-height: 120px;
  }

  .create-card:hover {
    background-color: #383838;
    border-color: #1DB954;
    transform: translateY(-2px);
  }

  .create-icon {
    color: #888;
    transition: color 0.2s;
  }

  .create-card:hover .create-icon {
    color: #1DB954;
  }

  .create-text {
    color: #888;
    font-weight: 500;
    transition: color 0.2s;
  }

  .create-card:hover .create-text {
    color: white;
  }
</style>
