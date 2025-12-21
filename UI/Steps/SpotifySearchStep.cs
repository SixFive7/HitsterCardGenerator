using Spectre.Console;
using Spectre.Console.Rendering;
using HitsterCardGenerator.Models;
using HitsterCardGenerator.Services;

namespace HitsterCardGenerator.UI.Steps;

/// <summary>
/// Search status for a song during Spotify lookup
/// </summary>
public enum SearchStatus
{
    Pending,
    Found,
    NotFound,
    Skipped,
    ManualSelection
}

/// <summary>
/// Tracks the search result for a song
/// </summary>
public class SongSearchResult
{
    public Song Song { get; set; } = null!;
    public SearchStatus Status { get; set; } = SearchStatus.Pending;
    public SpotifySearchResult? Match { get; set; }
}

public static class SpotifySearchStep
{
    /// <summary>
    /// Processes all songs, searching Spotify and applying smart selection or manual fallback
    /// </summary>
    public static async Task<List<SongSearchResult>> ProcessSongsAsync(
        List<Song> songs,
        SpotifyService service,
        Action<IRenderable, IRenderable> renderCallback,
        IRenderable stepsPanel)
    {
        var results = songs.Select(s => new SongSearchResult { Song = s }).ToList();

        for (int i = 0; i < results.Count; i++)
        {
            var result = results[i];
            var song = result.Song;

            // Update progress panel
            var progressPanel = GetProgressPanel(i + 1, results.Count, $"{song.Artist} - {song.Title}", "Searching...");
            renderCallback(stepsPanel, progressPanel);

            // Search Spotify
            var searchResults = await service.SearchTrackAsync(song.Artist, song.Title);

            if (searchResults.Count == 0)
            {
                result.Status = SearchStatus.NotFound;
                continue;
            }

            // Try smart selection
            var bestMatch = service.SelectBestMatch(searchResults, song.Artist, song.Title);

            if (bestMatch != null)
            {
                result.Match = bestMatch;
                result.Status = SearchStatus.Found;
            }
            else
            {
                // Ambiguous - need manual selection
                result.Status = SearchStatus.ManualSelection;

                // Show selection UI
                var selectionPanel = GetSelectionPanel(song, searchResults);
                renderCallback(stepsPanel, selectionPanel);

                var selected = PromptForSelection(searchResults);
                if (selected != null)
                {
                    result.Match = selected;
                    result.Status = SearchStatus.Found;
                }
                else
                {
                    result.Status = SearchStatus.Skipped;
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Gets the progress panel showing current search state
    /// </summary>
    public static IRenderable GetProgressPanel(int current, int total, string currentSong, string status)
    {
        var progress = (double)current / total;
        var percentage = (int)(progress * 100);
        var barWidth = 30;
        var filledWidth = (int)(barWidth * progress);
        var progressBar = new string('█', filledWidth) + new string('░', barWidth - filledWidth);

        var content = new Rows(
            new Markup($"[bold]Searching Spotify for tracks...[/]\n"),
            new Markup($"[dim]Progress:[/] [{(percentage == 100 ? "green" : "yellow")}]{progressBar}[/] {percentage}%"),
            new Markup($"[dim]Song {current} of {total}[/]\n"),
            new Markup($"[bold]Current:[/] {Markup.Escape(currentSong)}"),
            new Markup($"[dim]Status:[/] [{GetStatusColor(status)}]{status}[/]")
        );

        return new Panel(content)
            .Header("[blue]Step 4: Spotify Search[/]")
            .Border(BoxBorder.Rounded);
    }

    /// <summary>
    /// Gets the panel for manual selection when multiple matches found
    /// </summary>
    private static IRenderable GetSelectionPanel(Song song, List<SpotifySearchResult> options)
    {
        var table = new Table()
            .Border(TableBorder.Simple)
            .BorderColor(Color.Grey);

        table.AddColumn(new TableColumn("#").Width(3));
        table.AddColumn(new TableColumn("Track").Width(25));
        table.AddColumn(new TableColumn("Artist").Width(15));
        table.AddColumn(new TableColumn("Album").Width(20));
        table.AddColumn(new TableColumn("Type").Width(10));
        table.AddColumn(new TableColumn("Year").Width(5));

        for (int i = 0; i < options.Count; i++)
        {
            var opt = options[i];
            var typeColor = opt.AlbumType.ToLowerInvariant() switch
            {
                "album" => "green",
                "single" => "yellow",
                "compilation" => "red",
                _ => "dim"
            };

            table.AddRow(
                (i + 1).ToString(),
                Truncate(opt.TrackName, 23) + (opt.IsRemastered ? " [dim](RM)[/]" : ""),
                Truncate(opt.ArtistName, 13),
                Truncate(opt.AlbumName, 18),
                $"[{typeColor}]{opt.AlbumType}[/]",
                opt.ReleaseYear.ToString()
            );
        }

        var content = new Rows(
            new Markup($"[yellow]Multiple matches found for:[/] [bold]{Markup.Escape(song.Artist)} - {Markup.Escape(song.Title)}[/]\n"),
            new Markup("[dim]Select the correct track:[/]\n"),
            table
        );

        return new Panel(content)
            .Header("[blue]Step 4: Spotify Search - Manual Selection[/]")
            .Border(BoxBorder.Rounded);
    }

    /// <summary>
    /// Prompts user to select from multiple matches
    /// </summary>
    public static SpotifySearchResult? PromptForSelection(List<SpotifySearchResult> options)
    {
        var choices = options
            .Select((opt, i) => $"{i + 1}. {opt.TrackName} - {opt.ArtistName} ({opt.AlbumType}, {opt.ReleaseYear})")
            .ToList();
        choices.Add("Skip this song");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .PageSize(12)
                .AddChoices(choices));

        if (choice == "Skip this song")
            return null;

        var index = int.Parse(choice.Split('.')[0]) - 1;
        return options[index];
    }

    /// <summary>
    /// Gets the summary panel showing final results
    /// </summary>
    public static IRenderable GetSummaryPanel(List<SongSearchResult> results)
    {
        var found = results.Count(r => r.Status == SearchStatus.Found);
        var notFound = results.Count(r => r.Status == SearchStatus.NotFound);
        var skipped = results.Count(r => r.Status == SearchStatus.Skipped);

        var table = new Table()
            .Border(TableBorder.Simple)
            .BorderColor(Color.Grey);

        table.AddColumn(new TableColumn("#").Width(3));
        table.AddColumn(new TableColumn("Song").Width(30));
        table.AddColumn(new TableColumn("Status").Width(12));
        table.AddColumn(new TableColumn("Spotify Match").Width(30));

        var displayCount = Math.Min(results.Count, 15);
        for (int i = 0; i < displayCount; i++)
        {
            var r = results[i];
            var songName = Truncate($"{r.Song.Artist} - {r.Song.Title}", 28);
            var (statusText, statusColor) = r.Status switch
            {
                SearchStatus.Found => ("Found", "green"),
                SearchStatus.NotFound => ("Not Found", "red"),
                SearchStatus.Skipped => ("Skipped", "yellow"),
                _ => ("Pending", "dim")
            };

            var matchInfo = r.Match != null
                ? Truncate($"{r.Match.TrackName} ({r.Match.AlbumType})", 28)
                : "-";

            table.AddRow(
                (i + 1).ToString(),
                songName,
                $"[{statusColor}]{statusText}[/]",
                matchInfo
            );
        }

        if (results.Count > 15)
        {
            table.AddRow("", $"[dim]...and {results.Count - 15} more[/]", "", "");
        }

        var summaryText = $"[bold]Search Complete![/]\n\n" +
                          $"[green]Found:[/] {found}  |  [red]Not Found:[/] {notFound}  |  [yellow]Skipped:[/] {skipped}";

        var promptText = found > 0
            ? "\n[yellow]Select an action below:[/]"
            : "\n[red]No songs matched. Go back to fix your CSV or try different song entries.[/]";

        var content = new Rows(
            new Markup(summaryText + "\n"),
            table,
            new Markup(promptText)
        );

        return new Panel(content)
            .Header("[blue]Step 4: Spotify Search - Complete[/]")
            .Border(BoxBorder.Rounded);
    }

    /// <summary>
    /// Prompts user for action after search completes
    /// </summary>
    public static bool PromptForAction(int foundCount)
    {
        if (foundCount == 0)
        {
            AnsiConsole.Markup("[dim]Press Enter to go back...[/]");
            Console.ReadLine();
            return false;
        }

        var choices = new[]
        {
            $"Continue with {foundCount} matched songs",
            "Go back and select a different file"
        };

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(choices));

        return choice.StartsWith("Continue");
    }

    private static string Truncate(string value, int maxLength)
    {
        if (value.Length <= maxLength) return value;
        return value.Substring(0, maxLength - 2) + "..";
    }

    private static string GetStatusColor(string status) => status switch
    {
        "Searching..." => "yellow",
        "Found" => "green",
        "Not found" => "red",
        "Manual selection needed" => "blue",
        _ => "dim"
    };
}
