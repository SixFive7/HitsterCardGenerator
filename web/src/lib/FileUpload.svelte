<script lang="ts">
  import { uploadCsv } from './api'
  import type { CsvUploadResponse } from './types'

  // Props (Svelte 5 style)
  let { onuploaded }: { onuploaded: (response: CsvUploadResponse) => void } = $props()

  // State using Svelte 5 runes
  let isDragOver = $state(false)
  let isUploading = $state(false)
  let errorMessage = $state<string | null>(null)

  // Reference to hidden file input
  let fileInput: HTMLInputElement

  function handleDragOver(event: DragEvent) {
    event.preventDefault()
    isDragOver = true
  }

  function handleDragLeave(event: DragEvent) {
    event.preventDefault()
    isDragOver = false
  }

  async function handleDrop(event: DragEvent) {
    event.preventDefault()
    isDragOver = false

    const files = event.dataTransfer?.files
    if (files && files.length > 0) {
      await handleFile(files[0])
    }
  }

  function handleClick() {
    fileInput.click()
  }

  async function handleFileChange(event: Event) {
    const input = event.target as HTMLInputElement
    if (input.files && input.files.length > 0) {
      await handleFile(input.files[0])
    }
  }

  async function handleFile(file: File) {
    // Validate file type
    if (!file.name.endsWith('.csv')) {
      errorMessage = 'Please select a CSV file'
      return
    }

    errorMessage = null
    isUploading = true

    try {
      const response = await uploadCsv(file)
      onuploaded(response)
    } catch (error) {
      errorMessage = error instanceof Error ? error.message : 'Failed to upload file'
    } finally {
      isUploading = false
    }
  }
</script>

<div class="w-full max-w-2xl mx-auto">
  <!-- Drop Zone -->
  <div
    class="relative border-2 border-dashed rounded-2xl p-12 text-center transition-all cursor-pointer
      {isDragOver ? 'border-[#1DB954] bg-[#1DB954]/10' : 'border-gray-600 bg-[#282828] hover:border-gray-500'}"
    ondragover={handleDragOver}
    ondragleave={handleDragLeave}
    ondrop={handleDrop}
    onclick={handleClick}
    onkeydown={(e) => e.key === 'Enter' && handleClick()}
    role="button"
    tabindex="0"
  >
    {#if isUploading}
      <!-- Uploading State -->
      <div class="flex flex-col items-center gap-4">
        <div class="w-16 h-16 border-4 border-[#1DB954] border-t-transparent rounded-full animate-spin"></div>
        <p class="text-xl text-gray-300 font-medium">Processing...</p>
      </div>
    {:else if isDragOver}
      <!-- Drag Over State -->
      <div class="flex flex-col items-center gap-4">
        <div class="text-6xl">📁</div>
        <p class="text-2xl text-[#1DB954] font-medium">Drop to upload</p>
      </div>
    {:else}
      <!-- Default State -->
      <div class="flex flex-col items-center gap-4">
        <div class="text-6xl">📄</div>
        <p class="text-2xl text-gray-300 font-medium">Drop CSV file here or click to select</p>
        <p class="text-sm text-gray-500">Supports semicolon-separated format (title;artist;year;genre)</p>
      </div>
    {/if}

    <!-- Hidden File Input -->
    <input
      bind:this={fileInput}
      type="file"
      accept=".csv"
      onchange={handleFileChange}
      class="hidden"
    />
  </div>

  <!-- Error Message -->
  {#if errorMessage}
    <div class="mt-4 p-4 bg-[#FF6B6B]/20 border border-[#FF6B6B] rounded-lg">
      <p class="text-[#FF6B6B] font-medium">{errorMessage}</p>
    </div>
  {/if}
</div>
