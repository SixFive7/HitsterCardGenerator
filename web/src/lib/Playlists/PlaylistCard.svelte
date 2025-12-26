<script lang="ts">
  import type { Playlist } from '../types'

  interface Props {
    playlist: Playlist
    isSelected?: boolean
    onSelect: (id: string) => void
  }

  let { playlist, isSelected = false, onSelect }: Props = $props()

  function formatDate(dateString: string): string {
    const date = new Date(dateString)
    const now = new Date()
    const diffMs = now.getTime() - date.getTime()
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))

    if (diffDays === 0) {
      return 'Today'
    } else if (diffDays === 1) {
      return 'Yesterday'
    } else if (diffDays < 7) {
      return `${diffDays} days ago`
    } else {
      return date.toLocaleDateString()
    }
  }
</script>

<button
  onclick={() => onSelect(playlist.id)}
  class="playlist-card"
  class:selected={isSelected}
>
  <div class="icon">
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="w-8 h-8">
      <path fill-rule="evenodd" d="M19.952 1.651a.75.75 0 01.298.599V16.303a3 3 0 01-2.176 2.884l-1.32.377a2.553 2.553 0 11-1.403-4.909l2.311-.66a1.5 1.5 0 001.088-1.442V6.994l-9 2.572v9.737a3 3 0 01-2.176 2.884l-1.32.377a2.553 2.553 0 11-1.402-4.909l2.31-.66a1.5 1.5 0 001.088-1.442V5.25a.75.75 0 01.544-.721l10.5-3a.75.75 0 01.456.122z" clip-rule="evenodd" />
    </svg>
  </div>

  <div class="content">
    <h3 class="name">{playlist.name}</h3>
    <div class="meta">
      <span class="track-count">{playlist.trackCount} tracks</span>
      <span class="separator">-</span>
      <span class="updated">Updated {formatDate(playlist.updatedAt)}</span>
    </div>
  </div>

  {#if isSelected}
    <div class="selected-indicator">
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="w-5 h-5">
        <path fill-rule="evenodd" d="M2.25 12c0-5.385 4.365-9.75 9.75-9.75s9.75 4.365 9.75 9.75-4.365 9.75-9.75 9.75S2.25 17.385 2.25 12zm13.36-1.814a.75.75 0 10-1.22-.872l-3.236 4.53L9.53 12.22a.75.75 0 00-1.06 1.06l2.25 2.25a.75.75 0 001.14-.094l3.75-5.25z" clip-rule="evenodd" />
      </svg>
    </div>
  {/if}
</button>

<style>
  .playlist-card {
    display: flex;
    align-items: center;
    gap: 1rem;
    width: 100%;
    padding: 1.25rem;
    background-color: #282828;
    border: 2px solid transparent;
    border-radius: 0.75rem;
    cursor: pointer;
    transition: all 0.2s ease;
    text-align: left;
  }

  .playlist-card:hover {
    background-color: #383838;
    border-color: #484848;
    transform: translateY(-2px);
  }

  .playlist-card.selected {
    border-color: #1DB954;
    background-color: rgba(29, 185, 84, 0.1);
  }

  .playlist-card.selected:hover {
    background-color: rgba(29, 185, 84, 0.15);
  }

  .icon {
    flex-shrink: 0;
    width: 3rem;
    height: 3rem;
    display: flex;
    align-items: center;
    justify-content: center;
    background-color: #1DB954;
    border-radius: 0.5rem;
    color: white;
  }

  .content {
    flex: 1;
    min-width: 0;
  }

  .name {
    margin: 0;
    font-size: 1.125rem;
    font-weight: 600;
    color: white;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .meta {
    margin-top: 0.25rem;
    font-size: 0.875rem;
    color: #b3b3b3;
    display: flex;
    align-items: center;
    gap: 0.5rem;
  }

  .separator {
    color: #666;
  }

  .selected-indicator {
    flex-shrink: 0;
    color: #1DB954;
  }
</style>
