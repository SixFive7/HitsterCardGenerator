namespace HitsterCardGenerator.Models;

using LiteDB;

public class Playlist
{
    public ObjectId Id { get; set; } = ObjectId.NewObjectId();
    public string BrowserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<ObjectId> TrackIds { get; set; } = new();
}
