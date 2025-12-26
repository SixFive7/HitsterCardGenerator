namespace HitsterCardGenerator.Repositories;

using HitsterCardGenerator.Models;
using LiteDB;

public interface ITrackRepository
{
    Track? GetById(ObjectId id);
    Track? GetBySpotifyId(string spotifyId);
    IEnumerable<Track> GetByIds(IEnumerable<ObjectId> ids);
    ObjectId Create(Track track);
    ObjectId GetOrCreate(Track track);
    bool Delete(ObjectId id);
}
