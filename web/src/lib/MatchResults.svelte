<script lang="ts">
  import type { MatchResult, SpotifyMatch } from './types'

  // Props
  let {
    results,
    onSelectAlternative
  }: {
    results: MatchResult[]
    onSelectAlternative: (resultIndex: number, alternative: SpotifyMatch) => void
  } = $props()

  // State to track which result has alternatives panel open
  let expandedResultIndex = $state<number | null>(null)

  function toggleAlternatives(index: number) {
    expandedResultIndex = expandedResultIndex === index ? null : index
  }

  function selectAlternative(resultIndex: number, alternative: SpotifyMatch) {
    onSelectAlternative(resultIndex, alternative)
    expandedResultIndex = null // Close the alternatives panel
  }

  function getConfidenceBadge(confidence: 'high' | 'medium' | 'low' | 'none') {
    switch (confidence) {
      case 'high':
        return { color: 'bg-green-500', text: 'Exact' }
      case 'medium':
        return { color: 'bg-amber-500', text: 'Close' }
      case 'low':
        return { color: 'bg-red-500', text: 'Uncertain' }
      case 'none':
        return { color: 'bg-gray-500', text: 'Not Found' }
    }
  }
</script>

<div class="space-y-4">
  {#each results as result, index (index)}
    <div class="bg-[#282828] rounded-lg overflow-hidden">
      <!-- Main Match Display -->
      <div class="p-4">
        <div class="flex items-center gap-4">
          <!-- Album Art -->
          <div class="flex-shrink-0">
            {#if result.match}
              <img
                src={result.match.albumImageUrl}
                alt={result.match.albumName}
                class="w-16 h-16 rounded object-cover"
              />
            {:else}
              <div class="w-16 h-16 rounded bg-[#191414] flex items-center justify-center text-gray-600 text-2xl">
                ?
              </div>
            {/if}
          </div>

          <!-- Track Info -->
          <div class="flex-1 min-w-0">
            {#if result.match}
              <h3 class="text-white font-bold text-lg truncate">{result.match.trackName}</h3>
              <p class="text-gray-400 truncate">{result.match.artistName}</p>
              <p class="text-gray-500 text-sm truncate">{result.match.albumName}</p>
            {:else}
              <h3 class="text-gray-400 font-bold text-lg">Not found</h3>
              <p class="text-gray-500">{result.originalTitle}</p>
              <p class="text-gray-500 text-sm">{result.originalArtist} - {result.originalYear}</p>
            {/if}
          </div>

          <!-- Confidence Badge -->
          <div class="flex-shrink-0">
            {#if result.confidence !== 'none'}
              {@const badge = getConfidenceBadge(result.confidence)}
              <span class="{badge.color} text-white text-xs font-bold px-3 py-1 rounded-full">
                {badge.text}
              </span>
            {/if}
          </div>

          <!-- Change Button -->
          {#if result.alternatives.length > 0}
            <div class="flex-shrink-0">
              <button
                onclick={() => toggleAlternatives(index)}
                class="bg-[#191414] hover:bg-[#1DB954] text-white font-medium px-4 py-2 rounded-full transition-colors text-sm"
              >
                {expandedResultIndex === index ? 'Close' : 'Change'}
              </button>
            </div>
          {/if}
        </div>

        <!-- Original Song Info (subtle) -->
        <div class="mt-2 text-xs text-gray-600">
          Original: {result.originalTitle} - {result.originalArtist} ({result.originalYear})
        </div>
      </div>

      <!-- Alternatives Panel -->
      {#if expandedResultIndex === index && result.alternatives.length > 0}
        <div class="border-t border-gray-700 bg-[#191414] p-4">
          <p class="text-sm text-gray-400 mb-3 font-medium">Select an alternative:</p>
          <div class="space-y-2">
            {#each result.alternatives as alternative}
              <button
                onclick={() => selectAlternative(index, alternative)}
                class="w-full flex items-center gap-3 p-3 rounded-lg hover:bg-[#282828] transition-colors text-left"
              >
                <!-- Smaller Album Art -->
                <img
                  src={alternative.albumImageUrl}
                  alt={alternative.albumName}
                  class="w-10 h-10 rounded object-cover flex-shrink-0"
                />

                <!-- Track Info -->
                <div class="flex-1 min-w-0">
                  <p class="text-white font-medium truncate">{alternative.trackName}</p>
                  <p class="text-gray-400 text-sm truncate">{alternative.artistName}</p>
                  <p class="text-gray-500 text-xs truncate">{alternative.albumName}</p>
                </div>

                <!-- Select Icon -->
                <div class="flex-shrink-0 text-gray-500">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
                  </svg>
                </div>
              </button>
            {/each}
          </div>
        </div>
      {/if}
    </div>
  {/each}
</div>
