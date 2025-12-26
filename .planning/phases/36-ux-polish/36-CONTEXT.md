# Phase 36: UX Polish - Context

## Vision

Add the finishing touches to make the playlist management experience complete and polished. This includes proper error handling with toast notifications, loading states during API operations, and full playlist management (rename/delete).

## Essential

### Error Handling
- Toast notifications for API errors
- Clear error messages for common failures
- Non-blocking - user can dismiss and continue

### Loading States
- Loading spinner during playlist load
- Disabled buttons during save operations
- Visual feedback during API calls

### Playlist Management
- Rename playlist (edit name inline or via modal)
- Delete playlist with confirmation
- Update playlist list after changes

## UI Components

```
Toast.svelte - Toast notification component
ToastContainer.svelte - Container for positioning toasts
EditPlaylistModal.svelte - Modal for renaming playlist
DeleteConfirmModal.svelte - Confirmation dialog for delete
```

## Boundaries (Out of Scope)

- Track reordering within playlist
- Offline support / optimistic updates
- Undo functionality

## Notes

- Keep toast design consistent with existing UI
- Delete confirmation is important - prevent accidental loss
- Loading states should be subtle, not jarring
- Consider keyboard accessibility for modals
