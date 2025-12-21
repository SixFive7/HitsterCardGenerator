<script lang="ts">
  import { fetchHealth } from './lib/api'
  import { fly, fade } from 'svelte/transition'
  import FileUpload from './lib/FileUpload.svelte'
  import type { CsvUploadResponse } from './lib/types'

  // Svelte 5 runes
  let apiStatus = $state<'loading' | 'connected' | 'error'>('loading')
  let errorMessage = $state<string>('')
  let currentStep = $state<'landing' | 'upload' | 'results'>('landing')
  let uploadResult = $state<CsvUploadResponse | null>(null)

  // Fetch API health on mount
  $effect(() => {
    fetchHealth()
      .then(() => {
        apiStatus = 'connected'
      })
      .catch((error) => {
        apiStatus = 'error'
        errorMessage = error.message
      })
  })

  function handleGetStarted() {
    currentStep = 'upload'
  }

  function handleUploaded(response: CsvUploadResponse) {
    uploadResult = response
    currentStep = 'results'
  }

  function handleUploadDifferentFile() {
    currentStep = 'upload'
    uploadResult = null
  }
</script>

<main class="min-h-screen bg-gradient-to-br from-[#191414] via-[#282828] to-[#191414] p-8">
  <div class="max-w-4xl mx-auto">
    <!-- Landing Page -->
    {#if currentStep === 'landing'}
      <div in:fly={{ y: 50, duration: 800 }}>
        <!-- Main Title with Gradient -->
        <h1 class="text-7xl font-black text-center mb-6 bg-gradient-to-r from-[#1DB954] via-[#FF6B6B] to-[#1DB954] bg-clip-text text-transparent animate-gradient">
          Hitster Card Generator
        </h1>

        <!-- Animated Music Note -->
        <div class="flex justify-center mb-8">
          <div class="text-8xl animate-bounce">🎵</div>
        </div>

        <!-- Tagline -->
        <p class="text-2xl text-center text-gray-300 mb-12 font-light">
          Create stunning music quiz cards in seconds
        </p>

        <!-- API Status Indicator -->
        <div class="flex justify-center mb-8">
          {#if apiStatus === 'loading'}
            <div class="flex items-center gap-3 bg-[#282828] px-6 py-4 rounded-full shadow-lg" in:fade>
              <div class="w-3 h-3 bg-yellow-400 rounded-full animate-pulse"></div>
              <span class="text-yellow-400 font-medium">Connecting to backend...</span>
            </div>
          {:else if apiStatus === 'connected'}
            <div class="flex items-center gap-3 bg-[#282828] px-6 py-4 rounded-full shadow-lg" in:fly={{ y: 20, duration: 500 }}>
              <div class="w-3 h-3 bg-[#1DB954] rounded-full animate-pulse"></div>
              <span class="text-[#1DB954] font-medium">Connected to backend</span>
            </div>
          {:else}
            <div class="flex items-center gap-3 bg-[#282828] px-6 py-4 rounded-full shadow-lg" in:fly={{ y: 20, duration: 500 }}>
              <div class="w-3 h-3 bg-[#FF6B6B] rounded-full"></div>
              <span class="text-[#FF6B6B] font-medium">Connection failed: {errorMessage}</span>
            </div>
          {/if}
        </div>

        <!-- Get Started Button -->
        {#if apiStatus === 'connected'}
          <div class="flex justify-center mb-16" in:fly={{ y: 20, duration: 500, delay: 300 }}>
            <button
              onclick={handleGetStarted}
              class="bg-[#1DB954] hover:bg-[#1ed760] text-white font-bold text-xl px-12 py-4 rounded-full transform hover:scale-105 transition-all shadow-lg"
            >
              Get Started
            </button>
          </div>
        {/if}

        <!-- Feature Highlights -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div class="bg-[#282828] p-6 rounded-2xl text-center transform hover:scale-105 transition-transform" in:fly={{ y: 50, duration: 800, delay: 200 }}>
            <div class="text-5xl mb-4">🎨</div>
            <h3 class="text-xl font-bold text-[#1DB954] mb-2">Beautiful Design</h3>
            <p class="text-gray-400">Professionally styled cards ready to print</p>
          </div>
          <div class="bg-[#282828] p-6 rounded-2xl text-center transform hover:scale-105 transition-transform" in:fly={{ y: 50, duration: 800, delay: 400 }}>
            <div class="text-5xl mb-4">⚡</div>
            <h3 class="text-xl font-bold text-[#FF6B6B] mb-2">Lightning Fast</h3>
            <p class="text-gray-400">Generate dozens of cards in seconds</p>
          </div>
          <div class="bg-[#282828] p-6 rounded-2xl text-center transform hover:scale-105 transition-transform" in:fly={{ y: 50, duration: 800, delay: 600 }}>
            <div class="text-5xl mb-4">📄</div>
            <h3 class="text-xl font-bold text-[#1DB954] mb-2">PDF Export</h3>
            <p class="text-gray-400">Print-ready PDFs with perfect layout</p>
          </div>
        </div>
      </div>

    <!-- Upload Page -->
    {:else if currentStep === 'upload'}
      <div in:fly={{ y: 50, duration: 600 }}>
        <h2 class="text-5xl font-bold text-center mb-4 text-white">Upload Your Song List</h2>
        <p class="text-center text-gray-400 mb-12">Upload a CSV file with your songs (title;artist;year;genre)</p>

        <FileUpload onuploaded={handleUploaded} />

        <div class="mt-8 text-center">
          <button
            onclick={() => currentStep = 'landing'}
            class="text-gray-400 hover:text-white transition-colors"
          >
            ← Back to Home
          </button>
        </div>
      </div>

    <!-- Results Page -->
    {:else if currentStep === 'results' && uploadResult}
      <div in:fly={{ y: 50, duration: 600 }}>
        <h2 class="text-5xl font-bold text-center mb-8 text-white">Upload Results</h2>

        <!-- Summary -->
        <div class="bg-[#282828] rounded-2xl p-8 mb-8 text-center">
          <div class="text-6xl mb-4">{uploadResult.invalidSongs.length === 0 ? '✅' : '⚠️'}</div>
          <h3 class="text-3xl font-bold mb-4 text-white">{uploadResult.errorSummary}</h3>
          <div class="flex justify-center gap-8 text-xl">
            <div>
              <span class="text-gray-400">Total:</span>
              <span class="text-white font-bold ml-2">{uploadResult.totalSongs}</span>
            </div>
            <div>
              <span class="text-gray-400">Valid:</span>
              <span class="text-[#1DB954] font-bold ml-2">{uploadResult.validSongs.length}</span>
            </div>
            {#if uploadResult.invalidSongs.length > 0}
              <div>
                <span class="text-gray-400">Errors:</span>
                <span class="text-[#FF6B6B] font-bold ml-2">{uploadResult.invalidSongs.length}</span>
              </div>
            {/if}
          </div>
        </div>

        <!-- Invalid Songs (if any) -->
        {#if uploadResult.invalidSongs.length > 0}
          <div class="mb-8">
            <h3 class="text-2xl font-bold text-[#FF6B6B] mb-4">Songs with Errors</h3>
            <div class="space-y-3">
              {#each uploadResult.invalidSongs as song}
                <div class="bg-[#282828] border-l-4 border-[#FF6B6B] rounded-lg p-4">
                  <div class="flex justify-between items-start mb-2">
                    <div>
                      <p class="text-white font-bold">{song.title || '(No title)'}</p>
                      <p class="text-gray-400">{song.artist || '(No artist)'} - {song.year || 'No year'} - {song.genre || 'No genre'}</p>
                    </div>
                  </div>
                  <div class="mt-2 space-y-1">
                    {#each song.validationErrors as error}
                      <p class="text-[#FF6B6B] text-sm">• {error}</p>
                    {/each}
                  </div>
                </div>
              {/each}
            </div>
          </div>
        {/if}

        <!-- Valid Songs -->
        {#if uploadResult.validSongs.length > 0}
          <div class="mb-8">
            <h3 class="text-2xl font-bold text-[#1DB954] mb-4">Valid Songs</h3>
            <div class="bg-[#282828] rounded-lg overflow-hidden">
              <div class="overflow-x-auto">
                <table class="w-full">
                  <thead class="bg-[#191414]">
                    <tr>
                      <th class="px-4 py-3 text-left text-gray-400 font-medium">Title</th>
                      <th class="px-4 py-3 text-left text-gray-400 font-medium">Artist</th>
                      <th class="px-4 py-3 text-left text-gray-400 font-medium">Year</th>
                      <th class="px-4 py-3 text-left text-gray-400 font-medium">Genre</th>
                    </tr>
                  </thead>
                  <tbody>
                    {#each uploadResult.validSongs as song}
                      <tr class="border-t border-gray-700 hover:bg-[#1DB954]/10 transition-colors">
                        <td class="px-4 py-3 text-white">{song.title}</td>
                        <td class="px-4 py-3 text-gray-300">{song.artist}</td>
                        <td class="px-4 py-3 text-gray-300">{song.year}</td>
                        <td class="px-4 py-3 text-gray-300">{song.genre}</td>
                      </tr>
                    {/each}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        {/if}

        <!-- Action Buttons -->
        <div class="flex justify-center gap-4">
          <button
            onclick={handleUploadDifferentFile}
            class="bg-[#282828] hover:bg-[#383838] text-white font-bold px-8 py-3 rounded-full transition-all"
          >
            Upload Different File
          </button>
          {#if uploadResult.validSongs.length > 0}
            <button
              class="bg-[#1DB954] hover:bg-[#1ed760] text-white font-bold px-8 py-3 rounded-full transition-all opacity-50 cursor-not-allowed"
              disabled
            >
              Generate Cards (Coming Soon)
            </button>
          {/if}
        </div>
      </div>
    {/if}
  </div>
</main>

<style>
  @keyframes gradient {
    0%, 100% {
      background-position: 0% 50%;
    }
    50% {
      background-position: 100% 50%;
    }
  }

  .animate-gradient {
    background-size: 200% auto;
    animation: gradient 3s ease infinite;
  }
</style>
