namespace HitsterCardGenerator.Models;

using LiteDB;

public class Track
{
    public ObjectId Id { get; set; } = ObjectId.NewObjectId();
    public string SpotifyId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string? AlbumArtUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
