using Spectre.Console;
using Spectre.Console.Rendering;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.UI.Steps;

public static class ColorChoiceStep
{
    /// <summary>
    /// Gets the content panel explaining the color option
    /// </summary>
    public static IRenderable GetContentPanel()
    {
        // Build sample colors display
        var samples = GenreColors.GetSampleColors(6).ToList();
        var sampleText = string.Join("  ", samples.Select(s => $"[{s.Color}]\u2588\u2588[/] {s.Genre}"));

        var content = new Rows(
            new Markup("[bold]Background Colors[/]\n"),
            new Markup("You can add genre-based background colors to your cards.\n"),
            new Markup("Each genre has a unique color that will appear at 80% transparency.\n\n"),
            new Markup($"[dim]Sample colors:[/]\n{sampleText}\n\n"),
            new Markup("[yellow]Would you like to add genre-based background colors to cards?[/]")
        );

        return new Panel(content)
            .Header("[blue]Step 6: Color Choice[/]")
            .Border(BoxBorder.Rounded);
    }

    /// <summary>
    /// Prompts user for their color preference
    /// </summary>
    /// <returns>True if user wants background colors, false otherwise</returns>
    public static bool PromptForChoice()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(new[]
                {
                    "Yes - Add genre-based background colors",
                    "No - Keep cards with white backgrounds"
                }));

        return choice.StartsWith("Yes");
    }
}
