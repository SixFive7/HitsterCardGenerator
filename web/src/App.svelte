<script lang="ts">
  import { fetchHealth, matchSongs, fetchPlaylists, createPlaylist } from './lib/api'
  import { fly, fade } from 'svelte/transition'
  import FileUpload from './lib/FileUpload.svelte'
  import MatchResults from './lib/MatchResults.svelte'
  import CardCarousel from './lib/CardPreview/CardCarousel.svelte'
  import CardControls from './lib/CardPreview/CardControls.svelte'
  import GenreColorPicker from './lib/ColorSettings/GenreColorPicker.svelte'
  import ExportStep from './lib/ExportStep.svelte'
  import SpotifySearch from './lib/SpotifySearch.svelte'
  import PlaylistBuilder from './lib/PlaylistBuilder.svelte'
  import PlaylistList from './lib/Playlists/PlaylistList.svelte'
  import CreatePlaylistModal from './lib/Playlists/CreatePlaylistModal.svelte'
  import type { CsvUploadResponse, MatchResult, SpotifyMatch, PlaylistTrack, Playlist } from './lib/types'
  import {
    getCardCustomizationState,
    getGenreColor
  } from './lib/stores/cardCustomization.svelte'
  import {
    getPlaylistState,
    addTrack,
    removeTrack,
    updateTrackGenre,
    clearPlaylist
  } from './lib/stores/playlist.svelte'
  import {
    getSelectedPlaylistState,
    setSelectedPlaylistId
  } from './lib/stores/selectedPlaylist.svelte'

  // Svelte 5 runes
  let apiStatus = $state<'loading' | 'connected' | 'error'>('loading')
  let errorMessage = $state<string>('')
  let currentStep = $state<'landing' | 'upload' | 'build' | 'results' | 'matching' | 'matched' | 'preview' | 'export'>('landing')
  let flowMode = $state<'csv' | 'playlist' | null>(null)
  let uploadResult = $state<CsvUploadResponse | null>(null)
  let matchResults = $state<MatchResult[]>([])
  let isMatching = $state<boolean>(false)
  let matchError = $state<string | null>(null)

  // Card customization state
  const customizationState = getCardCustomizationState()
  let flippedCards = $state<Set<number>>(new Set())

  // Playlist state
  const playlistState = getPlaylistState()

  // Playlist selection state
  let playlists = $state<Playlist[]>([])
  let isLoadingPlaylists = $state<boolean>(true)
  let showCreateModal = $state<boolean>(false)
  const selectedPlaylistState = getSelectedPlaylistState()

  // Get currently selected playlist
  const selectedPlaylist = $derived(
    playlists.find(p => p.id === selectedPlaylistState.selectedPlaylistId) ?? null
  )

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

  // Fetch playlists on mount (after API is connected)
  $effect(() => {
    if (apiStatus === 'connected') {
      loadPlaylists()
    }
  })

  async function loadPlaylists() {
    isLoadingPlaylists = true
    try {
      const fetchedPlaylists = await fetchPlaylists()
      playlists = fetchedPlaylists

      if (fetchedPlaylists.length === 0) {
        // Auto-create "My Playlist" on first visit
        const newPlaylist = await createPlaylist('My Playlist')
        playlists = [newPlaylist]
        setSelectedPlaylistId(newPlaylist.id)
      } else {
        // Check if stored selection is still valid
        const storedId = selectedPlaylistState.selectedPlaylistId
        const storedExists = fetchedPlaylists.some(p => p.id === storedId)

        if (!storedExists) {
          // Select the first playlist
          setSelectedPlaylistId(fetchedPlaylists[0].id)
        }
      }
    } catch (error) {
      console.error('Failed to load playlists:', error)
    } finally {
      isLoadingPlaylists = false
    }
  }

  function handleSelectPlaylist(id: string) {
    setSelectedPlaylistId(id)
  }

  function handleOpenCreateModal() {
    showCreateModal = true
  }

  function handleCloseCreateModal() {
    showCreateModal = false
  }

  async function handleCreatePlaylist(name: string) {
    try {
      const newPlaylist = await createPlaylist(name)
      playlists = [...playlists, newPlaylist]
      setSelectedPlaylistId(newPlaylist.id)
      showCreateModal = false
    } catch (error) {
      console.error('Failed to create playlist:', error)
    }
  }

  function handleUploadCSV() {
    flowMode = 'csv'
    currentStep = 'upload'
  }

  function handleBuildPlaylist() {
    flowMode = 'playlist'
    currentStep = 'build'
  }

  function handleUploaded(response: CsvUploadResponse) {
    uploadResult = response
    currentStep = 'results'
  }

  function handleUploadDifferentFile() {
    currentStep = 'upload'
    uploadResult = null
    matchResults = []
    matchError = null
  }

  async function handleMatchWithSpotify() {
    if (!uploadResult || uploadResult.validSongs.length === 0) return

    isMatching = true
    matchError = null
    currentStep = 'matching'

    try {
      const response = await matchSongs(uploadResult.validSongs)
      matchResults = response.results
      currentStep = 'matched'
    } catch (error) {
      matchError = error instanceof Error ? error.message : 'Failed to match songs'
      currentStep = 'results'
    } finally {
      isMatching = false
    }
  }

  function handleSelectAlternative(resultIndex: number, alternative: SpotifyMatch) {
    // Update the match results with the selected alternative
    matchResults = matchResults.map((result, index) => {
      if (index === resultIndex) {
        return {
          ...result,
          match: alternative,
          confidence: 'high' as const // User-selected becomes high confidence
        }
      }
      return result
    })
  }

  // Convert PlaylistTrack[] to MatchResult[] for unified preview/export
  function playlistToMatchResults(tracks: PlaylistTrack[]): MatchResult[] {
    return tracks.map((track, index) => ({
      index,
      originalTitle: track.trackName,
      originalArtist: track.artistName,
      originalYear: track.releaseYear,
      originalGenre: track.genre,
      match: {
        trackId: track.trackId,
        trackName: track.trackName,
        artistName: track.artistName,
        albumName: track.albumName,
        albumImageUrl: track.albumImageUrl,
        spotifyUrl: track.spotifyUrl
      },
      confidence: 'high' as const,
      alternatives: []
    }))
  }

  function handleContinueToPreview() {
    // Reset current card index
    customizationState.currentCardIndex = 0
    // Clear flipped cards
    flippedCards = new Set()
    // Navigate to preview step
    currentStep = 'preview'
  }

  function handleContinueToPreviewFromBuild() {
    // Convert playlist tracks to match results
    matchResults = playlistToMatchResults(playlistState.tracks)
    // Reset current card index
    customizationState.currentCardIndex = 0
    // Clear flipped cards
    flippedCards = new Set()
    // Navigate to preview step
    currentStep = 'preview'
  }

  function handleBackToMatches() {
    // Flow-aware back navigation
    if (flowMode === 'playlist') {
      currentStep = 'build'
    } else {
      currentStep = 'matched'
    }
  }

  function handleContinueToExport() {
    currentStep = 'export'
  }

  function handleStartNewBatch() {
    // Reset all state
    uploadResult = null
    matchResults = []
    matchError = null
    flowMode = null
    clearPlaylist()
    // Navigate to landing
    currentStep = 'landing'
  }

  // Preview control handlers
  function handlePrevCard() {
    if (customizationState.currentCardIndex > 0) {
      customizationState.currentCardIndex--
    }
  }

  function handleNextCard() {
    if (customizationState.currentCardIndex < matchResults.length - 1) {
      customizationState.currentCardIndex++
    }
  }

  function handleFlipCard(index?: number) {
    const cardIndex = index !== undefined ? index : customizationState.currentCardIndex
    if (flippedCards.has(cardIndex)) {
      flippedCards.delete(cardIndex)
    } else {
      flippedCards.add(cardIndex)
    }
    flippedCards = new Set(flippedCards)
  }

  // Get unique genres from match results
  const uniqueGenres = $derived(
    Array.from(new Set(matchResults.map(r => r.originalGenre))).sort()
  )

  // Create a derived reactive object for genre colors from match results
  // This properly tracks store changes through getGenreColor() calls
  const reactiveGenreColors = $derived.by(() => {
    const colors: Record<string, string> = {}
    for (const result of matchResults) {
      if (result.originalGenre) {
        colors[result.originalGenre] = getGenreColor(result.originalGenre)
      }
    }
    return colors
  })
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

        <!-- Selected Playlist Indicator & Path Choice -->
        {#if apiStatus === 'connected'}
          <!-- Selected Playlist Indicator -->
          {#if selectedPlaylist}
            <div class="text-center mb-6" in:fly={{ y: 20, duration: 500, delay: 200 }}>
              <p class="text-gray-400 text-sm mb-1">Working with playlist:</p>
              <p class="text-[#1DB954] font-bold text-xl">{selectedPlaylist.name}</p>
            </div>
          {/if}

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-16" in:fly={{ y: 20, duration: 500, delay: 300 }}>
            <button
              onclick={handleUploadCSV}
              class="bg-[#282828] hover:bg-[#383838] p-8 rounded-2xl text-center transform hover:scale-105 transition-all cursor-pointer border-2 border-transparent hover:border-[#1DB954]"
            >
              <div class="text-6xl mb-4">📄</div>
              <h3 class="text-2xl font-bold text-white mb-2">Upload CSV</h3>
              <p class="text-gray-400">Import songs from a CSV file</p>
            </button>
            <button
              onclick={handleBuildPlaylist}
              class="bg-[#282828] hover:bg-[#383838] p-8 rounded-2xl text-center transform hover:scale-105 transition-all cursor-pointer border-2 border-transparent hover:border-[#1DB954]"
            >
              <div class="text-6xl mb-4">🎵</div>
              <h3 class="text-2xl font-bold text-white mb-2">Build Playlist</h3>
              <p class="text-gray-400">Search Spotify and build your list</p>
            </button>
          </div>

          <!-- Your Playlists Section -->
          <div class="mb-16" in:fly={{ y: 20, duration: 500, delay: 400 }}>
            <div class="flex items-center justify-between mb-6">
              <h2 class="text-2xl font-bold text-white">Your Playlists</h2>
              {#if isLoadingPlaylists}
                <div class="w-5 h-5 border-2 border-[#1DB954] border-t-transparent rounded-full animate-spin"></div>
              {/if}
            </div>

            {#if !isLoadingPlaylists}
              <PlaylistList
                {playlists}
                selectedId={selectedPlaylistState.selectedPlaylistId}
                onSelect={handleSelectPlaylist}
                onCreateNew={handleOpenCreateModal}
              />
            {:else}
              <div class="bg-[#282828] rounded-xl p-8 text-center">
                <p class="text-gray-400">Loading playlists...</p>
              </div>
            {/if}
          </div>
        {/if}

        <!-- Create Playlist Modal -->
        <CreatePlaylistModal
          isOpen={showCreateModal}
          onClose={handleCloseCreateModal}
          onCreate={handleCreatePlaylist}
        />

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

    <!-- Build Playlist Page -->
    {:else if currentStep === 'build'}
      <div in:fly={{ y: 50, duration: 600 }}>
        <h2 class="text-5xl font-bold text-center mb-4 text-white">Build Your Playlist</h2>
        <p class="text-center text-gray-400 mb-12">Search for tracks and add them to your playlist</p>

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
          <!-- Left: Spotify Search -->
          <div>
            <h3 class="text-2xl font-bold text-white mb-4">Search Spotify</h3>
            <SpotifySearch
              onAddTrack={addTrack}
              playlistTrackIds={new Set(playlistState.tracks.map(t => t.trackId))}
            />
          </div>

          <!-- Right: Playlist Builder -->
          <div>
            <PlaylistBuilder
              tracks={playlistState.tracks}
              onRemoveTrack={removeTrack}
              onUpdateGenre={updateTrackGenre}
              onContinueToPreview={handleContinueToPreviewFromBuild}
            />
          </div>
        </div>

        <div class="mt-8 text-center">
          <button
            onclick={() => { clearPlaylist(); currentStep = 'landing' }}
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
              onclick={handleMatchWithSpotify}
              class="bg-[#1DB954] hover:bg-[#1ed760] text-white font-bold px-8 py-3 rounded-full transition-all transform hover:scale-105"
            >
              Match with Spotify
            </button>
          {/if}
        </div>

        <!-- Error Message -->
        {#if matchError}
          <div class="mt-6 p-4 bg-[#FF6B6B]/20 border border-[#FF6B6B] rounded-lg text-center">
            <p class="text-[#FF6B6B] font-medium">{matchError}</p>
          </div>
        {/if}
      </div>

    <!-- Matching Progress Page -->
    {:else if currentStep === 'matching'}
      <div in:fly={{ y: 50, duration: 600 }}>
        <h2 class="text-5xl font-bold text-center mb-8 text-white">Matching with Spotify</h2>

        <div class="flex flex-col items-center gap-8 bg-[#282828] rounded-2xl p-12">
          <div class="w-24 h-24 border-4 border-[#1DB954] border-t-transparent rounded-full animate-spin"></div>
          <p class="text-2xl text-gray-300 font-medium">Searching Spotify for your songs...</p>
          <p class="text-gray-500">This may take a moment</p>
        </div>
      </div>

    <!-- Matched Results Page -->
    {:else if currentStep === 'matched'}
      <div in:fly={{ y: 50, duration: 600 }}>
        <h2 class="text-5xl font-bold text-center mb-8 text-white">Spotify Matches</h2>

        <!-- Summary Card -->
        <div class="bg-[#282828] rounded-2xl p-8 mb-8 text-center">
          <div class="text-6xl mb-4">🎵</div>
          <h3 class="text-3xl font-bold mb-4 text-white">Matching Complete</h3>
          <div class="flex justify-center gap-8 text-xl">
            <div>
              <span class="text-gray-400">Total:</span>
              <span class="text-white font-bold ml-2">{matchResults.length}</span>
            </div>
            <div>
              <span class="text-gray-400">Matched:</span>
              <span class="text-[#1DB954] font-bold ml-2">
                {matchResults.filter(r => r.match !== null).length}
              </span>
            </div>
            <div>
              <span class="text-gray-400">Not Found:</span>
              <span class="text-[#FF6B6B] font-bold ml-2">
                {matchResults.filter(r => r.match === null).length}
              </span>
            </div>
          </div>
        </div>

        <!-- Match Results -->
        <div class="mb-8">
          <MatchResults results={matchResults} onSelectAlternative={handleSelectAlternative} />
        </div>

        <!-- Action Buttons -->
        <div class="flex justify-center gap-4">
          <button
            onclick={handleUploadDifferentFile}
            class="bg-[#282828] hover:bg-[#383838] text-white font-bold px-8 py-3 rounded-full transition-all"
          >
            Re-upload CSV
          </button>
          <button
            onclick={handleContinueToPreview}
            class="bg-[#1DB954] hover:bg-[#1ed760] text-white font-bold px-8 py-3 rounded-full transition-all transform hover:scale-105"
          >
            Continue to Preview
          </button>
        </div>
      </div>

    <!-- Preview Page -->
    {:else if currentStep === 'preview'}
      <div in:fly={{ y: 50, duration: 600 }}>
        <h2 class="text-5xl font-bold text-center mb-8 text-white">Preview Your Cards</h2>

        <!-- Summary Bar -->
        <div class="bg-[#282828] rounded-2xl p-6 mb-8 text-center">
          <div class="flex justify-center gap-8 text-xl">
            <div>
              <span class="text-gray-400">Total Cards:</span>
              <span class="text-white font-bold ml-2">{matchResults.length}</span>
            </div>
          </div>
        </div>

        <!-- Two-column layout -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- Main: Card Preview -->
          <div class="lg:col-span-2">
            <CardCarousel
              cards={matchResults}
              genreColors={reactiveGenreColors}
              currentIndex={customizationState.currentCardIndex}
              onIndexChange={(index) => { customizationState.currentCardIndex = index }}
              flippedCards={flippedCards}
              onFlipToggle={handleFlipCard}
            />

            <CardControls
              totalCards={matchResults.length}
              currentIndex={customizationState.currentCardIndex}
              onPrev={handlePrevCard}
              onNext={handleNextCard}
              onFlip={handleFlipCard}
            />
          </div>

          <!-- Sidebar: Color Customization -->
          <div class="lg:col-span-1">
            <GenreColorPicker genres={uniqueGenres} />
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex justify-center gap-4 mt-8">
          <button
            onclick={handleBackToMatches}
            class="bg-[#282828] hover:bg-[#383838] text-white font-bold px-8 py-3 rounded-full transition-all"
          >
            Back to Matches
          </button>
          <button
            onclick={handleContinueToExport}
            disabled={matchResults.length === 0}
            class="bg-[#1DB954] hover:bg-[#1ed760] text-white font-bold px-8 py-3 rounded-full transition-all transform hover:scale-105 disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100"
          >
            Continue to Export ({matchResults.length} cards)
          </button>
        </div>
      </div>

    <!-- Export Page -->
    {:else if currentStep === 'export'}
      <div in:fly={{ y: 50, duration: 600 }}>
        <h2 class="text-5xl font-bold text-center mb-8 text-white">Export Your Cards</h2>

        <ExportStep
          matchResults={matchResults}
          genreColors={reactiveGenreColors}
          onStartNewBatch={handleStartNewBatch}
        />

        <!-- Back Button -->
        <div class="flex justify-center mt-8">
          <button
            onclick={() => currentStep = 'preview'}
            class="text-gray-400 hover:text-white transition-colors"
          >
            ← Back to Preview
          </button>
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
