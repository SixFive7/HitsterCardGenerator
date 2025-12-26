/**
 * Toast notification store using Svelte 5 runes
 * Manages toast notifications with auto-dismiss
 */

export type ToastType = 'success' | 'error' | 'info'

export interface Toast {
  id: string
  message: string
  type: ToastType
  duration: number
}

// Create the store state
let toasts = $state<Toast[]>([])
let nextId = 0

// Track timeouts for cleanup
const timeouts = new Map<string, ReturnType<typeof setTimeout>>()

/**
 * Shows a toast notification
 * @param message The message to display
 * @param type The type of toast (success, error, info)
 * @param duration Auto-dismiss duration in milliseconds (default: 5000)
 * @returns The toast ID
 */
export function showToast(message: string, type: ToastType = 'info', duration: number = 5000): string {
  const id = `toast-${nextId++}`

  const toast: Toast = {
    id,
    message,
    type,
    duration
  }

  toasts = [...toasts, toast]

  // Set up auto-dismiss
  if (duration > 0) {
    const timeout = setTimeout(() => {
      dismissToast(id)
    }, duration)
    timeouts.set(id, timeout)
  }

  return id
}

/**
 * Dismisses a toast by ID
 */
export function dismissToast(id: string): void {
  // Clear timeout if exists
  const timeout = timeouts.get(id)
  if (timeout) {
    clearTimeout(timeout)
    timeouts.delete(id)
  }

  toasts = toasts.filter(t => t.id !== id)
}

/**
 * Clears all toasts
 */
export function clearAllToasts(): void {
  // Clear all timeouts
  timeouts.forEach(timeout => clearTimeout(timeout))
  timeouts.clear()

  toasts = []
}

/**
 * Export reactive state
 */
export function getToastState() {
  return {
    get toasts() { return toasts }
  }
}
