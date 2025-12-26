namespace HitsterCardGenerator.Repositories;

using HitsterCardGenerator.Data;
using HitsterCardGenerator.Models;
using LiteDB;

public class TrackRepository : ITrackRepository
{
    private readonly ILiteDbContext _context;

    public TrackRepository(ILiteDbContext context)
    {
        _context = context;
    }

    private ILiteCollection<Track> Collection =>
        _context.Database.GetCollection<Track>("tracks");

    public Track? GetById(ObjectId id)
    {
        return Collection.FindById(id);
    }

    public Track? GetBySpotifyId(string spotifyId)
    {
        return Collection.FindOne(t => t.SpotifyId == spotifyId);
    }

    public IEnumerable<Track> GetByIds(IEnumerable<ObjectId> ids)
    {
        var idList = ids.ToList();
        return Collection.Find(t => idList.Contains(t.Id));
    }

    public ObjectId Create(Track track)
    {
        track.CreatedAt = DateTime.UtcNow;
        Collection.Insert(track);
        return track.Id;
    }

    public ObjectId GetOrCreate(Track track)
    {
        // Check if track with same SpotifyId exists
        var existing = GetBySpotifyId(track.SpotifyId);
        if (existing != null)
        {
            return existing.Id;
        }
        return Create(track);
    }

    public bool Delete(ObjectId id)
    {
        return Collection.Delete(id);
    }
}
