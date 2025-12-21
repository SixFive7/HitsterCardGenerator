namespace HitsterCardGenerator.Models;

/// <summary>
/// Maps genres to their associated background colors
/// </summary>
public static class GenreColors
{
    /// <summary>
    /// Genre to hex color mapping (35 genres, visually distinct colors)
    /// </summary>
    private static readonly Dictionary<string, string> Colors = new(StringComparer.OrdinalIgnoreCase)
    {
        // Popular genres (30)
        { "Rock", "#E63946" },         // Red
        { "Pop", "#FF69B4" },          // Hot pink
        { "Hip-Hop", "#FFD700" },      // Gold
        { "R&B", "#9B59B6" },          // Purple
        { "Country", "#D2691E" },      // Chocolate brown
        { "Jazz", "#6B5B95" },         // Violet
        { "Blues", "#4169E1" },        // Royal blue
        { "Electronic", "#00CED1" },   // Dark turquoise
        { "Dance", "#FF1493" },        // Deep pink
        { "House", "#32CD32" },        // Lime green
        { "Techno", "#008B8B" },       // Dark cyan
        { "Classical", "#1E3A5F" },    // Navy blue
        { "Reggae", "#228B22" },       // Forest green
        { "Soul", "#8B0000" },         // Dark red
        { "Funk", "#FF8C00" },         // Dark orange
        { "Disco", "#DA70D6" },        // Orchid
        { "Metal", "#2F4F4F" },        // Dark slate gray
        { "Punk", "#FF00FF" },         // Magenta
        { "Alternative", "#2E8B57" },  // Sea green
        { "Indie", "#DAA520" },        // Goldenrod
        { "Folk", "#808000" },         // Olive
        { "Latin", "#FF6347" },        // Tomato
        { "Rap", "#B8860B" },          // Dark goldenrod
        { "Gospel", "#FFE4B5" },       // Moccasin
        { "World", "#8B4513" },        // Saddle brown
        { "Ambient", "#87CEEB" },      // Sky blue
        { "New Wave", "#7B68EE" },     // Medium slate blue
        { "Grunge", "#556B2F" },       // Dark olive green
        { "Ska", "#20B2AA" },          // Light sea green
        { "Synthpop", "#FF1493" },     // Deep pink

        // French genres (5)
        { "Chanson", "#0055A4" },      // French blue
        { "Variete Francaise", "#3B5998" }, // Facebook blue (neutral)
        { "French Pop", "#FF69B4" },   // Hot pink (matches Pop)
        { "French Hip-Hop", "#FFD700" }, // Gold (matches Hip-Hop)
        { "Musette", "#EF4135" }       // French red
    };

    /// <summary>
    /// Gets the hex color for a genre
    /// </summary>
    /// <param name="genre">The genre name</param>
    /// <returns>Hex color string, or gray if genre not found</returns>
    public static string GetColor(string genre)
    {
        return Colors.TryGetValue(genre, out var color) ? color : "#808080";
    }

    /// <summary>
    /// Gets the color for a genre with alpha transparency for backgrounds
    /// </summary>
    /// <param name="genre">The genre name</param>
    /// <param name="alpha">Alpha value 0.0-1.0 (default 0.8 = 80%)</param>
    /// <returns>RGBA color string</returns>
    public static string GetColorWithAlpha(string genre, double alpha = 0.8)
    {
        var hex = GetColor(genre);

        // Convert hex to RGB
        var r = Convert.ToInt32(hex.Substring(1, 2), 16);
        var g = Convert.ToInt32(hex.Substring(3, 2), 16);
        var b = Convert.ToInt32(hex.Substring(5, 2), 16);

        return $"rgba({r},{g},{b},{alpha:F2})";
    }

    /// <summary>
    /// Gets a sample of genre colors for display
    /// </summary>
    public static IEnumerable<(string Genre, string Color)> GetSampleColors(int count = 6)
    {
        return Colors.Take(count).Select(kvp => (kvp.Key, kvp.Value));
    }
}
