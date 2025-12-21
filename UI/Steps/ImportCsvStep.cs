using Spectre.Console;
using Spectre.Console.Rendering;

namespace HitsterCardGenerator.UI.Steps;

public static class ImportCsvStep
{
    /// <summary>
    /// Returns the content panel with instructions AND prompt text included.
    /// </summary>
    public static IRenderable GetContentPanel(string? errorMessage = null)
    {
        var content =
            "[bold]Welcome to the Hitster Card Generator![/]\n\n" +
            "This wizard will guide you through creating custom Hitster cards.\n\n" +
            "[yellow]Expected CSV Format:[/]\n" +
            "• Header row: [cyan]title;artist;year;genre[/]\n" +
            "• Data rows: semicolon-separated values\n" +
            "• Year: 1900-2099\n" +
            "• Genre: Must be a valid genre (e.g., Rock, Pop, Jazz, etc.)\n\n" +
            "[dim]Example:[/]\n" +
            "[dim]title;artist;year;genre[/]\n" +
            "[dim]Bohemian Rhapsody;Queen;1975;Rock[/]\n" +
            "[dim]Billie Jean;Michael Jackson;1982;Pop[/]\n\n";

        if (!string.IsNullOrEmpty(errorMessage))
        {
            content += $"[red]{errorMessage}[/]\n\n";
        }

        content += "[yellow]Enter the path to your CSV file below:[/]";

        return new Panel(content)
            .Header("[blue]Step 1: Import CSV File[/]")
            .Border(BoxBorder.Rounded);
    }

    /// <summary>
    /// Prompts for file path. Returns path and error message if invalid.
    /// </summary>
    public static (string? path, string? error) PromptForFile()
    {
        var filePath = AnsiConsole.Prompt(
            new TextPrompt<string>("> ")
                .PromptStyle("green")
                .AllowEmpty());

        if (string.IsNullOrWhiteSpace(filePath))
        {
            return (null, "File path cannot be empty. Please try again.");
        }

        if (!File.Exists(filePath))
        {
            return (null, $"File not found: '{filePath}'. Please check the path.");
        }

        return (filePath, null);
    }
}
