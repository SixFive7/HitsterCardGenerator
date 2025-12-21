using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.Services;

public class CsvParser
{
    private readonly GenreValidator _genreValidator;

    public CsvParser(GenreValidator genreValidator)
    {
        _genreValidator = genreValidator;
    }

    /// <summary>
    /// Parses a CSV file with semicolon-separated values
    /// Expected format: title;artist;year;genre (header row required)
    /// </summary>
    /// <param name="filePath">Path to the CSV file</param>
    /// <returns>CsvParseResult with all songs and validation results</returns>
    public CsvParseResult ParseFile(string filePath)
    {
        // Check if file exists
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"CSV file not found: {filePath}", filePath);
        }

        var songs = new List<Song>();
        var lines = File.ReadAllLines(filePath);

        // Handle empty file
        if (lines.Length == 0)
        {
            return new CsvParseResult { Songs = songs };
        }

        // Parse header row
        var headerLine = lines[0].Trim();
        if (!IsValidHeader(headerLine))
        {
            // Create a dummy song with header error
            var headerError = new Song
            {
                ValidationErrors = new List<string>
                {
                    $"Invalid header format. Expected 'title;artist;year;genre' (case-insensitive), got '{headerLine}'"
                }
            };
            songs.Add(headerError);
            return new CsvParseResult { Songs = songs };
        }

        // Parse data rows (skip header)
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            // Skip empty lines
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Parse the line
            var song = Song.Parse(line, i + 1);

            // Additional genre validation (always validate genre, even if other errors exist)
            if (!string.IsNullOrWhiteSpace(song.Genre))
            {
                if (!_genreValidator.IsValidGenre(song.Genre))
                {
                    var closestMatch = _genreValidator.GetClosestMatch(song.Genre);
                    var errorMessage = $"Line {i + 1}: Invalid genre '{song.Genre}'.";

                    if (closestMatch != null)
                    {
                        errorMessage += $" Did you mean '{closestMatch}'?";
                    }
                    else
                    {
                        errorMessage += $" Valid genres: {string.Join(", ", Models.Genre.ValidGenres.OrderBy(g => g).Take(10))}...";
                    }

                    song.ValidationErrors.Add(errorMessage);
                }
                else if (song.ValidationErrors.Count == 0)
                {
                    // Only normalize genre if there are no other errors
                    var normalizedGenre = _genreValidator.NormalizeGenre(song.Genre);
                    if (normalizedGenre != song.Genre)
                    {
                        song = song with { Genre = normalizedGenre };
                    }
                }
            }

            songs.Add(song);
        }

        return new CsvParseResult { Songs = songs };
    }

    /// <summary>
    /// Validates that the header row has the expected format
    /// </summary>
    private bool IsValidHeader(string headerLine)
    {
        var parts = headerLine.Split(';');

        if (parts.Length < 4)
            return false;

        // Check for expected columns (case-insensitive)
        var expectedColumns = new[] { "title", "artist", "year", "genre" };
        for (int i = 0; i < 4; i++)
        {
            if (!parts[i].Trim().Equals(expectedColumns[i], StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }
}
