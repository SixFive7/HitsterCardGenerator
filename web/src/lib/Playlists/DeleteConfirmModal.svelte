<script lang="ts">
  import { fade, fly } from 'svelte/transition'
  import type { Playlist } from '../types'

  interface Props {
    isOpen: boolean
    playlist: Playlist | null
    canDelete: boolean
    onClose: () => void
    onConfirm: (id: string) => void
  }

  let { isOpen, playlist, canDelete, onClose, onConfirm }: Props = $props()

  function handleConfirm() {
    if (playlist && canDelete) {
      onConfirm(playlist.id)
    }
  }

  function handleKeydown(event: KeyboardEvent) {
    if (event.key === 'Escape') {
      onClose()
    }
  }
</script>

{#if isOpen && playlist}
  <!-- svelte-ignore a11y_no_noninteractive_element_interactions -->
  <!-- svelte-ignore a11y_interactive_supports_focus -->
  <div
    class="modal-backdrop"
    role="dialog"
    aria-modal="true"
    aria-labelledby="delete-modal-title"
    onclick={onClose}
    onkeydown={handleKeydown}
    transition:fade={{ duration: 200 }}
  >
    <!-- svelte-ignore a11y_click_events_have_key_events -->
    <!-- svelte-ignore a11y_no_static_element_interactions -->
    <div
      class="modal"
      onclick={(e) => e.stopPropagation()}
      transition:fly={{ y: -20, duration: 200 }}
    >
      <!-- Warning Icon -->
      <div class="warning-icon">
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126zM12 15.75h.007v.008H12v-.008z" />
        </svg>
      </div>

      <h2 id="delete-modal-title" class="modal-title">Delete Playlist</h2>

      {#if canDelete}
        <p class="modal-message">
          Are you sure you want to delete <strong>"{playlist.name}"</strong>?
        </p>
        <p class="modal-detail">
          This playlist contains {playlist.trackCount} track{playlist.trackCount !== 1 ? 's' : ''}.
          This action cannot be undone.
        </p>
      {:else}
        <p class="modal-message warning-text">
          Cannot delete the last playlist
        </p>
        <p class="modal-detail">
          You must have at least one playlist. Create another playlist before deleting this one.
        </p>
      {/if}

      <div class="actions">
        <button
          type="button"
          onclick={onClose}
          class="button button-secondary"
        >
          Cancel
        </button>
        {#if canDelete}
          <button
            type="button"
            onclick={handleConfirm}
            class="button button-danger"
          >
            Delete
          </button>
        {/if}
      </div>
    </div>
  </div>
{/if}

<style>
  .modal-backdrop {
    position: fixed;
    inset: 0;
    background-color: rgba(0, 0, 0, 0.75);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 50;
    padding: 1rem;
  }

  .modal {
    background-color: #282828;
    border-radius: 1rem;
    padding: 2rem;
    width: 100%;
    max-width: 420px;
    box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
    text-align: center;
  }

  .warning-icon {
    display: flex;
    justify-content: center;
    margin-bottom: 1rem;
  }

  .warning-icon svg {
    width: 3rem;
    height: 3rem;
    color: #FF6B6B;
  }

  .modal-title {
    margin: 0 0 1rem 0;
    font-size: 1.5rem;
    font-weight: 700;
    color: white;
  }

  .modal-message {
    margin: 0 0 0.5rem 0;
    font-size: 1rem;
    color: #e0e0e0;
  }

  .modal-message strong {
    color: white;
  }

  .modal-detail {
    margin: 0 0 1.5rem 0;
    font-size: 0.875rem;
    color: #888;
  }

  .warning-text {
    color: #FF6B6B;
    font-weight: 600;
  }

  .actions {
    display: flex;
    gap: 0.75rem;
    justify-content: center;
  }

  .button {
    padding: 0.75rem 1.5rem;
    border: none;
    border-radius: 0.5rem;
    font-size: 0.95rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
  }

  .button-secondary {
    background-color: #383838;
    color: white;
  }

  .button-secondary:hover {
    background-color: #484848;
  }

  .button-danger {
    background-color: #FF6B6B;
    color: white;
  }

  .button-danger:hover {
    background-color: #ff8585;
  }
</style>
