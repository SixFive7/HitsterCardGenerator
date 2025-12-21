using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.Services;

public class GenreValidator
{
    /// <summary>
    /// Checks if a genre is valid (case-insensitive)
    /// </summary>
    public bool IsValidGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            return false;

        return Genre.ValidGenres.Contains(genre);
    }

    /// <summary>
    /// Normalizes a genre to its properly-cased version from ValidGenres
    /// </summary>
    public string NormalizeGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            return genre;

        // Find the properly-cased version
        var normalized = Genre.ValidGenres.FirstOrDefault(g =>
            g.Equals(genre, StringComparison.OrdinalIgnoreCase));

        return normalized ?? genre;
    }

    /// <summary>
    /// Gets the closest matching genre for a given input using simple string similarity
    /// Returns null if no close match is found
    /// </summary>
    public string? GetClosestMatch(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            return null;

        genre = genre.ToLowerInvariant();

        // First, check for contains matches
        var containsMatch = Genre.ValidGenres.FirstOrDefault(g =>
            g.ToLowerInvariant().Contains(genre) || genre.Contains(g.ToLowerInvariant()));

        if (containsMatch != null)
            return containsMatch;

        // Use Levenshtein distance for close matches
        var bestMatch = Genre.ValidGenres
            .Select(g => new { Genre = g, Distance = LevenshteinDistance(genre.ToLowerInvariant(), g.ToLowerInvariant()) })
            .OrderBy(x => x.Distance)
            .FirstOrDefault();

        // Only suggest if distance is reasonable (less than half the length of the shorter string)
        if (bestMatch != null && bestMatch.Distance <= Math.Min(genre.Length, bestMatch.Genre.Length) / 2)
        {
            return bestMatch.Genre;
        }

        return null;
    }

    /// <summary>
    /// Calculates the Levenshtein distance between two strings
    /// </summary>
    private int LevenshteinDistance(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1))
            return s2?.Length ?? 0;

        if (string.IsNullOrEmpty(s2))
            return s1.Length;

        var d = new int[s1.Length + 1, s2.Length + 1];

        for (int i = 0; i <= s1.Length; i++)
            d[i, 0] = i;

        for (int j = 0; j <= s2.Length; j++)
            d[0, j] = j;

        for (int j = 1; j <= s2.Length; j++)
        {
            for (int i = 1; i <= s1.Length; i++)
            {
                int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        return d[s1.Length, s2.Length];
    }
}
