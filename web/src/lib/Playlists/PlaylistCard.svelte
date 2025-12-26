<script lang="ts">
  import type { Playlist } from '../types'

  interface Props {
    playlist: Playlist
    isSelected?: boolean
    onSelect: (id: string) => void
    onEdit?: (playlist: Playlist) => void
    onDelete?: (playlist: Playlist) => void
  }

  let { playlist, isSelected = false, onSelect, onEdit, onDelete }: Props = $props()

  function handleEdit(event: MouseEvent) {
    event.stopPropagation()
    onEdit?.(playlist)
  }

  function handleDelete(event: MouseEvent) {
    event.stopPropagation()
    onDelete?.(playlist)
  }

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

<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<div
  onclick={() => onSelect(playlist.id)}
  class="playlist-card"
  class:selected={isSelected}
  role="button"
  tabindex="0"
  onkeydown={(e) => e.key === 'Enter' && onSelect(playlist.id)}
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

  <div class="actions">
    {#if onEdit}
      <button
        onclick={handleEdit}
        class="action-button"
        aria-label="Edit playlist"
        title="Rename playlist"
      >
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="action-icon">
          <path stroke-linecap="round" stroke-linejoin="round" d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L10.582 16.07a4.5 4.5 0 01-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 011.13-1.897l8.932-8.931zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0115.75 21H5.25A2.25 2.25 0 013 18.75V8.25A2.25 2.25 0 015.25 6H10" />
        </svg>
      </button>
    {/if}
    {#if onDelete}
      <button
        onclick={handleDelete}
        class="action-button action-button-danger"
        aria-label="Delete playlist"
        title="Delete playlist"
      >
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="action-icon">
          <path stroke-linecap="round" stroke-linejoin="round" d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0" />
        </svg>
      </button>
    {/if}
  </div>

  {#if isSelected}
    <div class="selected-indicator">
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="w-5 h-5">
        <path fill-rule="evenodd" d="M2.25 12c0-5.385 4.365-9.75 9.75-9.75s9.75 4.365 9.75 9.75-4.365 9.75-9.75 9.75S2.25 17.385 2.25 12zm13.36-1.814a.75.75 0 10-1.22-.872l-3.236 4.53L9.53 12.22a.75.75 0 00-1.06 1.06l2.25 2.25a.75.75 0 001.14-.094l3.75-5.25z" clip-rule="evenodd" />
      </svg>
    </div>
  {/if}
</div>

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

  .actions {
    display: flex;
    gap: 0.25rem;
    flex-shrink: 0;
  }

  .action-button {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 2rem;
    height: 2rem;
    padding: 0;
    background: transparent;
    border: none;
    border-radius: 0.375rem;
    color: #888;
    cursor: pointer;
    transition: all 0.2s;
  }

  .action-button:hover {
    background-color: rgba(255, 255, 255, 0.1);
    color: white;
  }

  .action-button-danger:hover {
    background-color: rgba(255, 107, 107, 0.2);
    color: #FF6B6B;
  }

  .action-icon {
    width: 1.125rem;
    height: 1.125rem;
  }

  .selected-indicator {
    flex-shrink: 0;
    color: #1DB954;
  }
</style>
