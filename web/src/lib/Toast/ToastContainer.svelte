<script lang="ts">
  import { getToastState, dismissToast } from '../stores/toast.svelte'
  import Toast from './Toast.svelte'

  const toastState = getToastState()
</script>

<div class="toast-container" aria-live="polite" aria-label="Notifications">
  {#each toastState.toasts as toast (toast.id)}
    <Toast
      id={toast.id}
      message={toast.message}
      type={toast.type}
      duration={toast.duration}
      onDismiss={dismissToast}
    />
  {/each}
</div>

<style>
  .toast-container {
    position: fixed;
    top: 1.5rem;
    right: 1.5rem;
    z-index: 100;
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
    pointer-events: none;
  }

  .toast-container > :global(*) {
    pointer-events: auto;
  }

  @media (max-width: 640px) {
    .toast-container {
      top: 1rem;
      right: 1rem;
      left: 1rem;
    }
  }
</style>
