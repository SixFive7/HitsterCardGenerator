namespace HitsterCardGenerator.Models;

public class CsvParseResult
{
    /// <summary>
    /// All parsed songs (valid and invalid)
    /// </summary>
    public List<Song> Songs { get; init; } = new();

    /// <summary>
    /// Songs with no validation errors
    /// </summary>
    public List<Song> ValidSongs => Songs.Where(s => s.ValidationErrors.Count == 0).ToList();

    /// <summary>
    /// Songs with one or more validation errors
    /// </summary>
    public List<Song> InvalidSongs => Songs.Where(s => s.ValidationErrors.Count > 0).ToList();

    /// <summary>
    /// True if there are any invalid songs
    /// </summary>
    public bool HasErrors => InvalidSongs.Count > 0;

    /// <summary>
    /// Summary of parsing results
    /// </summary>
    public string ErrorSummary
    {
        get
        {
            if (Songs.Count == 0)
                return "No songs found in CSV file";

            if (!HasErrors)
                return $"All {Songs.Count} songs are valid";

            return $"{InvalidSongs.Count} of {Songs.Count} songs have validation errors";
        }
    }
}
