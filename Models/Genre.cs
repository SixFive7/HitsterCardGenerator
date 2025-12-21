namespace HitsterCardGenerator.Models;

public static class Genre
{
    /// <summary>
    /// Popular genres (30+ common music genres)
    /// </summary>
    public static readonly HashSet<string> AllGenres = new(StringComparer.OrdinalIgnoreCase)
    {
        "Rock",
        "Pop",
        "Hip-Hop",
        "R&B",
        "Country",
        "Jazz",
        "Blues",
        "Electronic",
        "Dance",
        "House",
        "Techno",
        "Classical",
        "Reggae",
        "Soul",
        "Funk",
        "Disco",
        "Metal",
        "Punk",
        "Alternative",
        "Indie",
        "Folk",
        "Latin",
        "Rap",
        "Gospel",
        "World",
        "Ambient",
        "New Wave",
        "Grunge",
        "Ska",
        "Synthpop"
    };

    /// <summary>
    /// French-specific genres (5 genres)
    /// </summary>
    public static readonly HashSet<string> FrenchGenres = new(StringComparer.OrdinalIgnoreCase)
    {
        "Chanson",
        "Variete Francaise",
        "French Pop",
        "French Hip-Hop",
        "Musette"
    };

    /// <summary>
    /// Combined set of all valid genres (35+ genres total)
    /// </summary>
    public static readonly HashSet<string> ValidGenres;

    static Genre()
    {
        ValidGenres = new HashSet<string>(AllGenres.Union(FrenchGenres), StringComparer.OrdinalIgnoreCase);
    }
}
