<script lang="ts">
  import { fade, fly } from 'svelte/transition'

  interface Props {
    isOpen: boolean
    onClose: () => void
    onCreate: (name: string) => void
  }

  let { isOpen, onClose, onCreate }: Props = $props()

  let playlistName = $state('My Playlist')
  let inputElement: HTMLInputElement | undefined = $state()

  // Focus input when modal opens
  $effect(() => {
    if (isOpen && inputElement) {
      // Small delay to ensure DOM is ready
      setTimeout(() => {
        inputElement?.focus()
        inputElement?.select()
      }, 50)
    }
  })

  // Reset name when modal closes
  $effect(() => {
    if (!isOpen) {
      playlistName = 'My Playlist'
    }
  })

  function handleSubmit(event: Event) {
    event.preventDefault()
    if (playlistName.trim()) {
      onCreate(playlistName.trim())
    }
  }

  function handleKeydown(event: KeyboardEvent) {
    if (event.key === 'Escape') {
      onClose()
    }
  }

  const isValid = $derived(playlistName.trim().length > 0)
</script>

{#if isOpen}
  <!-- svelte-ignore a11y_no_noninteractive_element_interactions -->
  <!-- svelte-ignore a11y_interactive_supports_focus -->
  <div
    class="modal-backdrop"
    role="dialog"
    aria-modal="true"
    aria-labelledby="modal-title"
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
      <h2 id="modal-title" class="modal-title">Create New Playlist</h2>

      <form onsubmit={handleSubmit}>
        <div class="form-group">
          <label for="playlist-name" class="label">Playlist Name</label>
          <input
            id="playlist-name"
            type="text"
            bind:value={playlistName}
            bind:this={inputElement}
            class="input"
            placeholder="Enter playlist name"
            maxlength="100"
          />
        </div>

        <div class="actions">
          <button
            type="button"
            onclick={onClose}
            class="button button-secondary"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={!isValid}
            class="button button-primary"
          >
            Create
          </button>
        </div>
      </form>
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
  }

  .modal-title {
    margin: 0 0 1.5rem 0;
    font-size: 1.5rem;
    font-weight: 700;
    color: white;
    text-align: center;
  }

  .form-group {
    margin-bottom: 1.5rem;
  }

  .label {
    display: block;
    margin-bottom: 0.5rem;
    font-size: 0.875rem;
    font-weight: 500;
    color: #b3b3b3;
  }

  .input {
    width: 100%;
    padding: 0.875rem 1rem;
    background-color: #383838;
    border: 2px solid #484848;
    border-radius: 0.5rem;
    color: white;
    font-size: 1rem;
    transition: all 0.2s;
    box-sizing: border-box;
  }

  .input:focus {
    outline: none;
    border-color: #1DB954;
    box-shadow: 0 0 0 3px rgba(29, 185, 84, 0.15);
  }

  .input::placeholder {
    color: #888;
  }

  .actions {
    display: flex;
    gap: 0.75rem;
    justify-content: flex-end;
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

  .button-primary {
    background-color: #1DB954;
    color: white;
  }

  .button-primary:hover:not(:disabled) {
    background-color: #1ed760;
  }

  .button-primary:disabled {
    background-color: #383838;
    color: #666;
    cursor: not-allowed;
  }
</style>
