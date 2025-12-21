namespace HitsterCardGenerator.Models;

public record Song
{
    public string Title { get; init; } = string.Empty;
    public string Artist { get; init; } = string.Empty;
    public int Year { get; init; }
    public string Genre { get; init; } = string.Empty;
    public string? SpotifyTrackId { get; init; }
    public List<string> ValidationErrors { get; init; } = new();

    /// <summary>
    /// Parses a CSV line (semicolon-separated) into a Song record.
    /// Expected format: title;artist;year;genre
    /// </summary>
    /// <param name="csvLine">The CSV line to parse</param>
    /// <param name="lineNumber">The line number for error reporting</param>
    /// <returns>A Song record with ValidationErrors populated if issues found</returns>
    public static Song Parse(string csvLine, int lineNumber = 0)
    {
        var errors = new List<string>();
        var parts = csvLine.Split(';');

        // Check column count
        if (parts.Length < 4)
        {
            errors.Add($"Line {lineNumber}: Expected 4 fields (title;artist;year;genre), found {parts.Length}");
            return new Song { ValidationErrors = errors };
        }

        var title = parts[0].Trim();
        var artist = parts[1].Trim();
        var yearStr = parts[2].Trim();
        var genre = parts[3].Trim();

        // Validate title
        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add($"Line {lineNumber}: Title is required");
        }

        // Validate artist
        if (string.IsNullOrWhiteSpace(artist))
        {
            errors.Add($"Line {lineNumber}: Artist is required");
        }

        // Validate year
        int year = 0;
        if (string.IsNullOrWhiteSpace(yearStr))
        {
            errors.Add($"Line {lineNumber}: Year is required");
        }
        else if (!int.TryParse(yearStr, out year))
        {
            errors.Add($"Line {lineNumber}: Year must be a valid number (got '{yearStr}')");
        }
        else if (year < 1900 || year > 2099)
        {
            errors.Add($"Line {lineNumber}: Year must be between 1900 and 2099 (got {year})");
        }

        // Validate genre (basic check - detailed validation happens in GenreValidator)
        if (string.IsNullOrWhiteSpace(genre))
        {
            errors.Add($"Line {lineNumber}: Genre is required");
        }

        return new Song
        {
            Title = title,
            Artist = artist,
            Year = year,
            Genre = genre,
            ValidationErrors = errors
        };
    }
}
