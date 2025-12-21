<script lang="ts">
  import { exportPdf } from './api'
  import type { MatchResult, CuttingLineStyle, ExportRequest } from './types'
  import { fade, fly } from 'svelte/transition'

  interface Props {
    matchResults: MatchResult[]
    genreColors: Record<string, string>
    onStartNewBatch: () => void
  }

  let { matchResults, genreColors, onStartNewBatch }: Props = $props()

  // State
  let cuttingLineStyle = $state<CuttingLineStyle>('EdgeOnly')
  let isDownloading = $state<boolean>(false)
  let downloadSuccess = $state<boolean>(false)
  let downloadFilename = $state<string>('')
  let downloadError = $state<string | null>(null)

  // Load cutting line preference from localStorage
  $effect(() => {
    if (typeof window !== 'undefined') {
      const stored = localStorage.getItem('hitster-cutting-lines')
      if (stored === 'EdgeOnly' || stored === 'Complete' || stored === 'None') {
        cuttingLineStyle = stored
      }
    }
  })

  // Save cutting line preference to localStorage
  function saveCuttingLinePreference(style: CuttingLineStyle) {
    if (typeof window !== 'undefined') {
      localStorage.setItem('hitster-cutting-lines', style)
    }
  }

  // Computed stats
  const cardCount = $derived(matchResults.length)
  const uniqueGenres = $derived(
    Array.from(new Set(matchResults.map(r => r.originalGenre))).length
  )
  const estimatedPages = $derived(Math.ceil(cardCount / 10) * 2)

  // Handle cutting line change
  function handleCuttingLineChange(style: CuttingLineStyle) {
    cuttingLineStyle = style
    saveCuttingLinePreference(style)
  }

  // Handle download
  async function handleDownload() {
    isDownloading = true
    downloadError = null

    try {
      // Build export request
      const exportRequest: ExportRequest = {
        cards: matchResults.map(result => ({
          trackId: result.match!.trackId,
          title: result.match!.trackName,
          artist: result.match!.artistName,
          year: result.originalYear,
          genre: result.originalGenre
        })),
        genreColors: genreColors,
        cuttingLines: cuttingLineStyle
      }

      // Call API
      const blob = await exportPdf(exportRequest)

      // Generate filename
      const now = new Date()
      const dateStr = now.toISOString().split('T')[0]
      const filename = `hitster-cards-${dateStr}-${cardCount}cards.pdf`
      downloadFilename = filename

      // Trigger browser download
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = filename
      document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      document.body.removeChild(a)

      // Show success
      downloadSuccess = true
    } catch (error) {
      downloadError = error instanceof Error ? error.message : 'Failed to export PDF'
    } finally {
      isDownloading = false
    }
  }
</script>

<div class="space-y-8">
  <!-- Summary Section -->
  <div class="bg-[#282828] rounded-2xl p-8">
    <h3 class="text-2xl font-bold text-white mb-6 text-center">Export Summary</h3>
    <div class="grid grid-cols-3 gap-6 text-center">
      <div>
        <div class="text-5xl font-bold text-[#1DB954] mb-2">{cardCount}</div>
        <div class="text-gray-400">Cards</div>
      </div>
      <div>
        <div class="text-5xl font-bold text-[#FF6B6B] mb-2">{uniqueGenres}</div>
        <div class="text-gray-400">Genres</div>
      </div>
      <div>
        <div class="text-5xl font-bold text-white mb-2">{estimatedPages}</div>
        <div class="text-gray-400">Pages (front/back)</div>
      </div>
    </div>
  </div>

  <!-- Cutting Lines Section -->
  {#if !downloadSuccess}
    <div class="bg-[#282828] rounded-2xl p-8" in:fade>
      <h3 class="text-xl font-bold text-white mb-4">Cutting Lines</h3>
      <p class="text-gray-400 mb-4 text-sm">Choose how cutting guides should appear on the PDF</p>

      <div class="space-y-3">
        <label class="flex items-center gap-3 p-4 bg-[#191414] rounded-lg cursor-pointer hover:bg-[#1DB954]/10 transition-colors border-2 {cuttingLineStyle === 'EdgeOnly' ? 'border-[#1DB954]' : 'border-transparent'}">
          <input
            type="radio"
            name="cuttingLines"
            value="EdgeOnly"
            checked={cuttingLineStyle === 'EdgeOnly'}
            onchange={() => handleCuttingLineChange('EdgeOnly')}
            class="w-5 h-5 text-[#1DB954] focus:ring-[#1DB954] focus:ring-2"
          />
          <div>
            <div class="text-white font-medium">Edge Lines Only</div>
            <div class="text-gray-400 text-sm">Minimal lines at page edges for ruler-guided cutting</div>
          </div>
        </label>

        <label class="flex items-center gap-3 p-4 bg-[#191414] rounded-lg cursor-pointer hover:bg-[#1DB954]/10 transition-colors border-2 {cuttingLineStyle === 'Complete' ? 'border-[#1DB954]' : 'border-transparent'}">
          <input
            type="radio"
            name="cuttingLines"
            value="Complete"
            checked={cuttingLineStyle === 'Complete'}
            onchange={() => handleCuttingLineChange('Complete')}
            class="w-5 h-5 text-[#1DB954] focus:ring-[#1DB954] focus:ring-2"
          />
          <div>
            <div class="text-white font-medium">Complete Grid Lines</div>
            <div class="text-gray-400 text-sm">Full grid surrounding each card for precise cutting</div>
          </div>
        </label>
      </div>
    </div>
  {/if}

  <!-- Download Section -->
  {#if !downloadSuccess}
    <div class="flex justify-center" in:fade>
      <button
        onclick={handleDownload}
        disabled={isDownloading}
        class="bg-[#1DB954] hover:bg-[#1ed760] text-white font-bold text-xl px-16 py-5 rounded-full transition-all transform hover:scale-105 shadow-lg disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100 flex items-center gap-3"
      >
        {#if isDownloading}
          <div class="w-6 h-6 border-3 border-white border-t-transparent rounded-full animate-spin"></div>
          <span>Generating PDF...</span>
        {:else}
          <span>Download PDF</span>
        {/if}
      </button>
    </div>
  {/if}

  <!-- Error Message -->
  {#if downloadError}
    <div class="p-4 bg-[#FF6B6B]/20 border border-[#FF6B6B] rounded-lg text-center" in:fly={{ y: 20 }}>
      <p class="text-[#FF6B6B] font-medium">{downloadError}</p>
    </div>
  {/if}

  <!-- Success State -->
  {#if downloadSuccess}
    <div class="bg-[#282828] rounded-2xl p-8 text-center space-y-6" in:fly={{ y: 50, duration: 600 }}>
      <div class="text-7xl">✓</div>
      <h3 class="text-3xl font-bold text-[#1DB954]">Download Started!</h3>
      <p class="text-gray-300">Your PDF has been generated and should start downloading</p>
      <div class="bg-[#191414] rounded-lg p-4 inline-block">
        <p class="text-white font-mono text-sm">{downloadFilename}</p>
      </div>

      <div class="pt-4">
        <button
          onclick={onStartNewBatch}
          class="bg-[#1DB954] hover:bg-[#1ed760] text-white font-bold text-lg px-12 py-4 rounded-full transition-all transform hover:scale-105"
        >
          Start New Batch
        </button>
      </div>
    </div>
  {/if}
</div>
