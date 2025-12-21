<script lang="ts">
  /**
   * GenreColorPicker - Per-genre color customization
   * Shows all genres with their colors and allows individual customization
   */
  import ColorPicker from 'svelte-awesome-color-picker'
  import ColorPalettes from './ColorPalettes.svelte'
  import { setGenreColor, getGenreColor, DEFAULT_GENRE_COLORS } from '../stores/cardCustomization.svelte'

  interface Props {
    genres: string[]
  }

  let { genres }: Props = $props()

  // State
  let selectedGenre = $state<string | null>(null)
  let selectedColor = $state<string>('#1DB954')

  // Select a genre to edit
  function selectGenre(genre: string) {
    selectedGenre = genre
    selectedColor = getGenreColor(genre)
  }

  // Update color when picker changes
  function handleColorChange(event: CustomEvent<string>) {
    if (selectedGenre) {
      const newColor = event.detail
      setGenreColor(selectedGenre, newColor)
      selectedColor = newColor
    }
  }

  // Apply a palette to all genres
  function handlePaletteSelect(paletteName: string) {
    const paletteData = getPaletteData(paletteName)

    // Apply colors to all genres in the palette
    Object.entries(paletteData).forEach(([genre, color]) => {
      setGenreColor(genre, color)
    })

    // Refresh selected color if a genre is selected
    if (selectedGenre) {
      selectedColor = getGenreColor(selectedGenre)
    }
  }

  // Get full palette data
  function getPaletteData(paletteName: string): Record<string, string> {
    const genreList = [
      "Rock", "Pop", "Hip-Hop", "R&B", "Country", "Jazz", "Blues", "Electronic",
      "Dance", "House", "Techno", "Classical", "Reggae", "Soul", "Funk", "Disco",
      "Metal", "Punk", "Alternative", "Indie", "Folk", "Latin", "Rap", "Gospel",
      "World", "Ambient", "New Wave", "Grunge", "Ska", "Synthpop",
      "Chanson", "Variete Francaise", "French Pop", "French Hip-Hop", "Musette"
    ]

    const paletteData: Record<string, string> = {}

    if (paletteName === 'neon') {
      const neonColors = [
        "#FF00FF", "#00FFFF", "#FF0080", "#00FF00", "#FFFF00", "#FF1493",
        "#7FFF00", "#00BFFF", "#FF4500", "#32CD32", "#FF69B4", "#1E90FF",
        "#FFD700", "#00FF7F", "#FF6347", "#9370DB", "#00CED1", "#FF00FF",
        "#ADFF2F", "#FF1493", "#00FA9A", "#FFA500", "#48D1CC", "#FF69B4",
        "#7CFC00", "#00FFFF", "#FF00FF", "#00FF00", "#FFFF00", "#FF0080",
        "#00BFFF", "#FF1493", "#32CD32", "#FF00FF", "#FFD700"
      ]
      genreList.forEach((genre, i) => {
        paletteData[genre] = neonColors[i % neonColors.length]
      })
    } else if (paletteName === 'pastel') {
      const pastelColors = [
        "#FFB3BA", "#BAE1FF", "#FFFFBA", "#D4A5D4", "#FFD8B4", "#C5C6F5",
        "#B4D4FF", "#E8D5E8", "#FFC9E3", "#D1FFD6", "#FFECB3", "#D4E5FF",
        "#FFD6E5", "#C8E6C9", "#FFE0B2", "#F8BBD0", "#C5CAE9", "#B2EBF2",
        "#F0F4C3", "#DCEDC8", "#FFE4E1", "#D7CCC8", "#CFD8DC", "#FFF9C4",
        "#F5F5DC", "#E6E6FA", "#FFDAB9", "#E0E0E0", "#F0E68C", "#DDA0DD",
        "#B0C4DE", "#FFE4E1", "#E6E6FA", "#FFDAB9", "#F5DEB3"
      ]
      genreList.forEach((genre, i) => {
        paletteData[genre] = pastelColors[i % pastelColors.length]
      })
    } else if (paletteName === 'grayscale') {
      const grayColors = [
        "#333333", "#555555", "#777777", "#999999", "#AAAAAA", "#444444",
        "#666666", "#888888", "#4D4D4D", "#707070", "#5C5C5C", "#808080",
        "#3A3A3A", "#626262", "#757575", "#4A4A4A", "#6E6E6E", "#838383",
        "#474747", "#696969", "#7C7C7C", "#505050", "#737373", "#8B8B8B",
        "#424242", "#656565", "#787878", "#4F4F4F", "#717171", "#858585",
        "#3E3E3E", "#676767", "#7A7A7A", "#525252", "#747474", "#878787"
      ]
      genreList.forEach((genre, i) => {
        paletteData[genre] = grayColors[i % grayColors.length]
      })
    } else {
      // Default Spotify palette
      return { ...DEFAULT_GENRE_COLORS }
    }

    return paletteData
  }
</script>

<div class="genre-color-picker">
  <!-- Palette Selection -->
  <ColorPalettes onSelectPalette={handlePaletteSelect} />

  <!-- Genre List -->
  <div class="genre-section">
    <h3 class="section-title">Genre Colors</h3>
    <div class="genre-list">
      {#each genres as genre}
        <button
          class="genre-item"
          class:selected={selectedGenre === genre}
          onclick={() => selectGenre(genre)}
        >
          <div class="genre-color-swatch" style="background-color: {getGenreColor(genre)};"></div>
          <span class="genre-name">{genre}</span>
        </button>
      {/each}
    </div>
  </div>

  <!-- Color Picker (shown when genre selected) -->
  {#if selectedGenre}
    <div class="color-picker-section">
      <h4 class="picker-title">Editing: {selectedGenre}</h4>
      <div class="color-picker-wrapper">
        <ColorPicker
          bind:hex={selectedColor}
          on:input={handleColorChange}
          label=""
          isAlpha={false}
          isTextInput={true}
          isInput={true}
          components={{
            wrapper: '#282828',
            slider: '#1DB954'
          }}
        />
      </div>
    </div>
  {/if}
</div>

<style>
  .genre-color-picker {
    background: #282828;
    border-radius: 16px;
    padding: 20px;
    color: white;
  }

  .section-title {
    font-size: 18px;
    font-weight: 600;
    color: white;
    margin-bottom: 12px;
  }

  .genre-section {
    margin-top: 24px;
  }

  .genre-list {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
    gap: 8px;
    max-height: 400px;
    overflow-y: auto;
    padding: 8px;
    background: #1a1a1a;
    border-radius: 8px;
  }

  .genre-item {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 10px 12px;
    background: #282828;
    border: 2px solid #333;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.2s ease;
    text-align: left;
  }

  .genre-item:hover {
    border-color: #1DB954;
    transform: translateX(4px);
  }

  .genre-item.selected {
    border-color: #1DB954;
    background: #1DB954;
    color: white;
  }

  .genre-color-swatch {
    width: 24px;
    height: 24px;
    border-radius: 4px;
    border: 2px solid rgba(255, 255, 255, 0.3);
    flex-shrink: 0;
  }

  .genre-name {
    font-size: 14px;
    font-weight: 500;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .color-picker-section {
    margin-top: 24px;
    padding: 16px;
    background: #1a1a1a;
    border-radius: 8px;
  }

  .picker-title {
    font-size: 16px;
    font-weight: 600;
    color: #1DB954;
    margin-bottom: 12px;
  }

  .color-picker-wrapper {
    display: flex;
    justify-content: center;
  }

  /* Custom scrollbar for genre list */
  .genre-list::-webkit-scrollbar {
    width: 8px;
  }

  .genre-list::-webkit-scrollbar-track {
    background: #1a1a1a;
    border-radius: 4px;
  }

  .genre-list::-webkit-scrollbar-thumb {
    background: #1DB954;
    border-radius: 4px;
  }

  .genre-list::-webkit-scrollbar-thumb:hover {
    background: #1ed760;
  }
</style>
