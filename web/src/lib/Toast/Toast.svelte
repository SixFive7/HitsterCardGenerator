<script lang="ts">
  import { fly, fade } from 'svelte/transition'
  import type { ToastType } from '../stores/toast.svelte'

  interface Props {
    id: string
    message: string
    type: ToastType
    duration: number
    onDismiss: (id: string) => void
  }

  let { id, message, type, duration, onDismiss }: Props = $props()

  // Icon SVG paths by type
  const icons: Record<ToastType, string> = {
    success: 'M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
    error: 'M12 9v3.75m9-.75a9 9 0 11-18 0 9 9 0 0118 0zm-9 3.75h.008v.008H12v-.008z',
    info: 'M11.25 11.25l.041-.02a.75.75 0 011.063.852l-.708 2.836a.75.75 0 001.063.853l.041-.021M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9-3.75h.008v.008H12V8.25z'
  }

  // Colors by type
  const colors: Record<ToastType, { bg: string; border: string; icon: string; progress: string }> = {
    success: {
      bg: 'rgba(29, 185, 84, 0.15)',
      border: '#1DB954',
      icon: '#1DB954',
      progress: '#1DB954'
    },
    error: {
      bg: 'rgba(255, 107, 107, 0.15)',
      border: '#FF6B6B',
      icon: '#FF6B6B',
      progress: '#FF6B6B'
    },
    info: {
      bg: 'rgba(59, 130, 246, 0.15)',
      border: '#3B82F6',
      icon: '#3B82F6',
      progress: '#3B82F6'
    }
  }

  const color = $derived(colors[type])
</script>

<div
  class="toast"
  style="background-color: {color.bg}; border-color: {color.border};"
  in:fly={{ x: 300, duration: 300 }}
  out:fade={{ duration: 200 }}
  role="alert"
>
  <div class="toast-content">
    <svg
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      viewBox="0 0 24 24"
      stroke-width="1.5"
      stroke={color.icon}
      class="toast-icon"
    >
      <path stroke-linecap="round" stroke-linejoin="round" d={icons[type]} />
    </svg>

    <span class="toast-message">{message}</span>

    <button
      onclick={() => onDismiss(id)}
      class="toast-close"
      aria-label="Dismiss notification"
    >
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="close-icon">
        <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
      </svg>
    </button>
  </div>

  {#if duration > 0}
    <div
      class="progress-bar"
      style="background-color: {color.progress}; animation-duration: {duration}ms;"
    ></div>
  {/if}
</div>

<style>
  .toast {
    display: flex;
    flex-direction: column;
    min-width: 300px;
    max-width: 450px;
    border: 1px solid;
    border-radius: 0.75rem;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
    overflow: hidden;
    background-color: #282828;
  }

  .toast-content {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    padding: 1rem;
  }

  .toast-icon {
    flex-shrink: 0;
    width: 1.5rem;
    height: 1.5rem;
  }

  .toast-message {
    flex: 1;
    color: white;
    font-size: 0.95rem;
    line-height: 1.4;
  }

  .toast-close {
    flex-shrink: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 1.5rem;
    height: 1.5rem;
    padding: 0;
    background: transparent;
    border: none;
    border-radius: 0.25rem;
    color: #888;
    cursor: pointer;
    transition: color 0.2s, background-color 0.2s;
  }

  .toast-close:hover {
    color: white;
    background-color: rgba(255, 255, 255, 0.1);
  }

  .close-icon {
    width: 1rem;
    height: 1rem;
  }

  .progress-bar {
    height: 3px;
    animation: shrink linear forwards;
  }

  @keyframes shrink {
    from {
      width: 100%;
    }
    to {
      width: 0%;
    }
  }
</style>
