<script lang="ts">
  /**
   * ColorPalettes - Preset genre color palettes
   * Allows quick application of predefined color schemes
   */

  interface Props {
    onSelectPalette: (paletteName: string) => void
  }

  let { onSelectPalette }: Props = $props()

  // Define preset palettes
  const palettes: Record<string, { name: string, colors: string[], description: string }> = {
    spotify: {
      name: "Spotify",
      colors: ["#E63946", "#FF69B4", "#FFD700", "#9B59B6"],
      description: "Default vibrant colors"
    },
    neon: {
      name: "Neon",
      colors: ["#FF00FF", "#00FFFF", "#FF0080", "#00FF00"],
      description: "Bright neon glow"
    },
    pastel: {
      name: "Pastel",
      colors: ["#FFB3BA", "#BAE1FF", "#FFFFBA", "#D4A5D4"],
      description: "Soft pastel shades"
    },
    grayscale: {
      name: "Grayscale",
      colors: ["#333333", "#555555", "#777777", "#999999"],
      description: "Classic monochrome"
    }
  }

  // Get full palette data for each palette name
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
      // Default Spotify palette - use DEFAULT_GENRE_COLORS
      const spotifyColors: Record<string, string> = {
        "Rock": "#E63946", "Pop": "#FF69B4", "Hip-Hop": "#FFD700", "R&B": "#9B59B6",
        "Country": "#D2691E", "Jazz": "#6B5B95", "Blues": "#4169E1", "Electronic": "#00CED1",
        "Dance": "#FF1493", "House": "#32CD32", "Techno": "#008B8B", "Classical": "#1E3A5F",
        "Reggae": "#228B22", "Soul": "#8B0000", "Funk": "#FF8C00", "Disco": "#DA70D6",
        "Metal": "#2F4F4F", "Punk": "#FF00FF", "Alternative": "#2E8B57", "Indie": "#DAA520",
        "Folk": "#808000", "Latin": "#FF6347", "Rap": "#B8860B", "Gospel": "#FFE4B5",
        "World": "#8B4513", "Ambient": "#87CEEB", "New Wave": "#7B68EE", "Grunge": "#556B2F",
        "Ska": "#20B2AA", "Synthpop": "#FF1493", "Chanson": "#0055A4",
        "Variete Francaise": "#3B5998", "French Pop": "#FF69B4", "French Hip-Hop": "#FFD700",
        "Musette": "#EF4135"
      }
      return spotifyColors
    }

    return paletteData
  }

  function handlePaletteClick(paletteName: string) {
    onSelectPalette(paletteName)
  }
</script>

<div class="palettes-container">
  <h3 class="palettes-title">Color Palettes</h3>
  <div class="palettes-grid">
    {#each Object.entries(palettes) as [key, palette]}
      <button
        class="palette-card"
        onclick={() => handlePaletteClick(key)}
        aria-label={`Apply ${palette.name} palette`}
      >
        <div class="palette-name">{palette.name}</div>
        <div class="palette-swatches">
          {#each palette.colors as color}
            <div class="swatch" style="background-color: {color};"></div>
          {/each}
        </div>
        <div class="palette-description">{palette.description}</div>
      </button>
    {/each}
  </div>
</div>

<style>
  .palettes-container {
    margin-bottom: 24px;
  }

  .palettes-title {
    font-size: 18px;
    font-weight: 600;
    color: white;
    margin-bottom: 12px;
  }

  .palettes-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
    gap: 12px;
  }

  .palette-card {
    background: #1a1a1a;
    border: 2px solid #333;
    border-radius: 12px;
    padding: 12px;
    cursor: pointer;
    transition: all 0.2s ease;
    text-align: center;
  }

  .palette-card:hover {
    border-color: #1DB954;
    transform: scale(1.05);
    box-shadow: 0 4px 12px rgba(29, 185, 84, 0.3);
  }

  .palette-name {
    font-size: 14px;
    font-weight: 600;
    color: white;
    margin-bottom: 8px;
  }

  .palette-swatches {
    display: flex;
    gap: 4px;
    justify-content: center;
    margin-bottom: 8px;
  }

  .swatch {
    width: 24px;
    height: 24px;
    border-radius: 4px;
    border: 1px solid rgba(255, 255, 255, 0.2);
  }

  .palette-description {
    font-size: 11px;
    color: #999;
  }
</style>
