namespace HitsterCardGenerator.Repositories;

using HitsterCardGenerator.Data;
using HitsterCardGenerator.Models;
using LiteDB;

public class PlaylistRepository : IPlaylistRepository
{
    private readonly ILiteDbContext _context;

    public PlaylistRepository(ILiteDbContext context)
    {
        _context = context;
    }

    private ILiteCollection<Playlist> Collection =>
        _context.Database.GetCollection<Playlist>("playlists");

    public Playlist? GetById(ObjectId id)
    {
        return Collection.FindById(id);
    }

    public IEnumerable<Playlist> GetByBrowserId(string browserId)
    {
        return Collection.Find(p => p.BrowserId == browserId);
    }

    public ObjectId Create(Playlist playlist)
    {
        playlist.CreatedAt = DateTime.UtcNow;
        playlist.UpdatedAt = DateTime.UtcNow;
        Collection.Insert(playlist);
        return playlist.Id;
    }

    public bool Update(Playlist playlist)
    {
        playlist.UpdatedAt = DateTime.UtcNow;
        return Collection.Update(playlist);
    }

    public bool Delete(ObjectId id)
    {
        return Collection.Delete(id);
    }
}
