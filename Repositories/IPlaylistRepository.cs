namespace HitsterCardGenerator.Repositories;

using HitsterCardGenerator.Models;
using LiteDB;

public interface IPlaylistRepository
{
    Playlist? GetById(ObjectId id);
    IEnumerable<Playlist> GetByBrowserId(string browserId);
    ObjectId Create(Playlist playlist);
    bool Update(Playlist playlist);
    bool Delete(ObjectId id);
}
