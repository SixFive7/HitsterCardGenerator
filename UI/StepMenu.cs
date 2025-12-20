using Spectre.Console;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.UI;

public static class StepMenu
{
    private static readonly Dictionary<Step, string> StepNames = new()
    {
        { Step.ImportCsv, "Import CSV" },
        { Step.ValidateCsv, "Validate CSV" },
        { Step.SpotifyAuth, "Spotify Auth" },
        { Step.SpotifySearch, "Spotify Search" },
        { Step.GenerateQR, "Generate QR Codes" },
        { Step.ColorChoice, "Color Choice" },
        { Step.DesignCards, "Design Cards" },
        { Step.ExportPdf, "Export PDF" }
    };

    public static Panel Render(WizardState state)
    {
        var table = new Table()
            .Border(TableBorder.None)
            .HideHeaders();

        table.AddColumn(new TableColumn("Step").Width(30));

        // Enumerate all steps
        var allSteps = Enum.GetValues<Step>();
        int stepNumber = 1;

        foreach (var step in allSteps)
        {
            var stepName = StepNames[step];
            string markup;

            if (state.IsCurrentStep(step))
            {
                // Current step: yellow with arrow
                markup = $"[yellow]→ {stepNumber}. {stepName}[/]";
            }
            else if (state.IsStepCompleted(step))
            {
                // Completed step: green with checkmark
                markup = $"[green]✓ {stepNumber}. {stepName}[/]";
            }
            else
            {
                // Pending step: dimmed
                markup = $"[grey]○ {stepNumber}. {stepName}[/]";
            }

            table.AddRow(markup);
            stepNumber++;
        }

        return new Panel(table)
            .Header("Steps")
            .Border(BoxBorder.Rounded);
    }
}
