<script lang="ts">
  /**
   * CardBack - Back face of the Hitster card
   * Displays server-rendered QuestPDF card image for pixel-perfect preview
   */
  import CardImage from './CardImage.svelte'

  interface Props {
    trackId: string
    title: string
    artist: string
    year: number
    genre: string
    backgroundColor: string
    albumImageUrl?: string
    albumName?: string
  }

  let { trackId, title, artist, year, genre, backgroundColor, albumImageUrl, albumName }: Props = $props()
</script>

<div class="card-back">
  <CardImage
    side="back"
    {trackId}
    {title}
    {artist}
    {year}
    {genre}
    {backgroundColor}
    {albumImageUrl}
    {albumName}
  />
</div>

<style>
  .card-back {
    width: 100%;
    height: 100%;
  }
</style>
