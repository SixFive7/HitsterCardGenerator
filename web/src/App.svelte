<script lang="ts">
  import { fetchHealth } from './lib/api'
  import { fly, fade } from 'svelte/transition'

  // Svelte 5 runes
  let apiStatus = $state<'loading' | 'connected' | 'error'>('loading')
  let errorMessage = $state<string>('')

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
</script>

<main class="min-h-screen bg-gradient-to-br from-[#191414] via-[#282828] to-[#191414] flex items-center justify-center p-8">
  <div class="max-w-4xl w-full" in:fly={{ y: 50, duration: 800 }}>
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
    <div class="flex justify-center">
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

    <!-- Feature Highlights -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mt-16">
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
