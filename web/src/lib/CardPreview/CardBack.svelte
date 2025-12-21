<script lang="ts">
  /**
   * CardBack - Back face of the Hitster card
   * Layout matching CardDesigner.cs:
   * - Artist (top, small)
   * - Year (center, large bold)
   * - Title (below year, medium)
   * - Genre (bottom, small)
   */
  interface Props {
    artist: string
    year: number
    title: string
    genre: string
    backgroundColor: string
  }

  let { artist, year, title, genre, backgroundColor }: Props = $props()

  // Helper to determine text color based on background brightness
  function getTextColor(bgColor: string): string {
    // Convert hex to RGB
    const hex = bgColor.replace('#', '')
    const r = parseInt(hex.substring(0, 2), 16)
    const g = parseInt(hex.substring(2, 4), 16)
    const b = parseInt(hex.substring(4, 6), 16)

    // Calculate brightness (perceived luminance)
    const brightness = (r * 299 + g * 587 + b * 114) / 1000

    // Return black for light backgrounds, white for dark backgrounds
    return brightness > 128 ? '#000000' : '#FFFFFF'
  }

  // Helper to get secondary text color (lighter/darker variant)
  function getSecondaryTextColor(bgColor: string): string {
    const textColor = getTextColor(bgColor)
    return textColor === '#000000' ? 'rgba(0, 0, 0, 0.6)' : 'rgba(255, 255, 255, 0.6)'
  }

  // Helper to get tertiary text color (even lighter/darker)
  function getTertiaryTextColor(bgColor: string): string {
    const textColor = getTextColor(bgColor)
    return textColor === '#000000' ? 'rgba(0, 0, 0, 0.4)' : 'rgba(255, 255, 255, 0.4)'
  }

  const primaryColor = $derived(getTextColor(backgroundColor))
  const secondaryColor = $derived(getSecondaryTextColor(backgroundColor))
  const tertiaryColor = $derived(getTertiaryTextColor(backgroundColor))
</script>

<div
  class="card-back"
  style="background-color: {backgroundColor}cc;"
>
  <div class="card-content">
    <!-- Artist (top, small) -->
    <div class="artist" style="color: {secondaryColor}">
      {artist}
    </div>

    <!-- Year (center, large and bold) -->
    <div class="year" style="color: {primaryColor}">
      {year}
    </div>

    <!-- Title (below year, medium) -->
    <div class="title" style="color: {secondaryColor}">
      {title}
    </div>

    <!-- Spacer -->
    <div class="spacer"></div>

    <!-- Genre (bottom, small) -->
    <div class="genre" style="color: {tertiaryColor}">
      {genre}
    </div>
  </div>
</div>

<style>
  .card-back {
    width: 100%;
    height: 100%;
    border-radius: 12px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    aspect-ratio: 17 / 11;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 16px;
    backface-visibility: hidden;
    transform: rotateY(180deg);
  }

  .card-content {
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 8px;
  }

  .artist {
    font-size: 14px;
    font-weight: 500;
    text-align: center;
    width: 100%;
  }

  .year {
    font-size: 56px;
    font-weight: 800;
    text-align: center;
    line-height: 1;
    margin: 16px 0;
  }

  .title {
    font-size: 18px;
    font-weight: 600;
    text-align: center;
    width: 100%;
    word-wrap: break-word;
  }

  .spacer {
    flex-grow: 1;
  }

  .genre {
    font-size: 12px;
    font-weight: 500;
    text-align: center;
    text-transform: uppercase;
    letter-spacing: 0.5px;
  }
</style>
