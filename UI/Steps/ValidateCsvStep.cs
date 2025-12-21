using Spectre.Console;
using Spectre.Console.Rendering;
using HitsterCardGenerator.Models;
using HitsterCardGenerator.Services;

namespace HitsterCardGenerator.UI.Steps;

public static class ValidateCsvStep
{
    private static CsvParseResult? _lastResult;

    /// <summary>
    /// Parses the CSV and returns the content panel to display inside the layout.
    /// </summary>
    public static IRenderable GetContentPanel(string filePath, CsvParser csvParser, out string? errorMessage)
    {
        errorMessage = null;

        try
        {
            _lastResult = csvParser.ParseFile(filePath);
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return new Panel($"[red]Error parsing CSV file:[/]\n{ex.Message}")
                .Header("[red]Validation Error[/]")
                .Border(BoxBorder.Rounded);
        }

        var result = _lastResult;
        var validCount = result.ValidSongs.Count;
        var errorCount = result.InvalidSongs.Count;
        var totalCount = result.Songs.Count;

        // Build prompt text based on validation result
        string promptText;
        if (errorCount == 0)
        {
            promptText = "\n[green]All songs are valid![/]\n[yellow]Select an action below:[/]";
        }
        else if (validCount == 0)
        {
            promptText = "\n[red]No valid songs to continue with.[/]\n[yellow]Press Enter to go back and fix your CSV.[/]";
        }
        else
        {
            promptText = $"\n[yellow]{errorCount} song(s) have validation errors.[/]\n[yellow]Select an action below:[/]";
        }

        // Build content with table and prompt
        var rows = new Rows(
            new Markup($"[bold]Total songs:[/] {totalCount}  |  [green]Valid:[/] {validCount}  |  [red]Errors:[/] {errorCount}\n"),
            BuildValidationTable(result),
            new Markup(promptText)
        );

        return new Panel(rows)
            .Header("[blue]Step 2: Validate CSV[/]")
            .Border(BoxBorder.Rounded);
    }

    private static Table BuildValidationTable(CsvParseResult result)
    {
        var table = new Table()
            .Border(TableBorder.Simple)
            .BorderColor(Color.Grey);

        table.AddColumn(new TableColumn("#").Width(3));
        table.AddColumn(new TableColumn("Title").Width(20));
        table.AddColumn(new TableColumn("Artist").Width(15));
        table.AddColumn(new TableColumn("Year").Width(5));
        table.AddColumn(new TableColumn("Genre").Width(12));
        table.AddColumn(new TableColumn("Status").Width(25));

        var displayCount = Math.Min(result.Songs.Count, 15);

        for (int i = 0; i < displayCount; i++)
        {
            var song = result.Songs[i];
            var title = Truncate(song.Title ?? "N/A", 18);
            var artist = Truncate(song.Artist ?? "N/A", 13);
            var year = song.Year > 0 ? song.Year.ToString() : "N/A";
            var genre = Truncate(song.Genre ?? "N/A", 10);

            string status;
            if (song.ValidationErrors.Count == 0)
            {
                status = "[green]Valid[/]";
            }
            else
            {
                var firstError = song.ValidationErrors.First();
                var cleanError = firstError.Contains(": ")
                    ? firstError.Substring(firstError.IndexOf(": ") + 2)
                    : firstError;
                status = $"[red]{Truncate(cleanError, 23)}[/]";
            }

            table.AddRow((i + 1).ToString(), title, artist, year, genre, status);
        }

        if (result.Songs.Count > 15)
        {
            table.AddRow("", $"[dim]...and {result.Songs.Count - 15} more[/]", "", "", "", "");
        }

        return table;
    }

    private static string Truncate(string value, int maxLength)
    {
        if (value.Length <= maxLength) return value;
        return value.Substring(0, maxLength - 2) + "..";
    }

    /// <summary>
    /// Prompts user for action based on validation results. Returns valid songs or empty list to go back.
    /// </summary>
    public static List<Song> PromptForAction()
    {
        if (_lastResult == null)
        {
            return new List<Song>();
        }

        var result = _lastResult;
        var validCount = result.ValidSongs.Count;
        var errorCount = result.InvalidSongs.Count;

        if (validCount == 0)
        {
            Console.ReadLine();
            return new List<Song>();
        }

        var choices = errorCount == 0
            ? new[] { "Continue with these songs", "Go back and select a different file" }
            : new[] { $"Continue with {validCount} valid songs", "Go back and fix the CSV file" };

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(choices));

        return choice.StartsWith("Continue") ? result.ValidSongs : new List<Song>();
    }
}
