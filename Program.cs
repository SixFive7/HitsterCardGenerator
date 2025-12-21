using Spectre.Console;
using Spectre.Console.Rendering;
using HitsterCardGenerator.UI;
using HitsterCardGenerator.Models;
using HitsterCardGenerator.UI.Steps;
using HitsterCardGenerator.Services;

// Create wizard state and app state
var wizardState = new WizardState();
var appState = new AppState();

// Create services
var genreValidator = new GenreValidator();
var csvParser = new CsvParser(genreValidator);

// Main wizard loop
while (wizardState.CurrentStep <= Step.ExportPdf)
{
    // Create step menu
    var stepsPanel = StepMenu.Render(wizardState);

    // Get content panel for current step
    IRenderable contentPanel;

    switch (wizardState.CurrentStep)
    {
        case Step.ImportCsv:
            // Loop until valid file is provided
            string? importError = null;
            while (true)
            {
                stepsPanel = StepMenu.Render(wizardState);
                contentPanel = ImportCsvStep.GetContentPanel(importError);
                AppLayout.Render(stepsPanel, contentPanel);

                var (path, error) = ImportCsvStep.PromptForFile();
                if (error != null)
                {
                    importError = error;
                    continue; // Re-render with error message
                }

                appState.CsvFilePath = path;
                break;
            }
            wizardState.AdvanceToNextStep();
            break;

        case Step.ValidateCsv:
            // Get content (parses CSV) and render layout
            contentPanel = ValidateCsvStep.GetContentPanel(appState.CsvFilePath!, csvParser, out var errorMessage);
            AppLayout.Render(stepsPanel, contentPanel);

            // If parse error, go back
            if (errorMessage != null)
            {
                AnsiConsole.Markup("[dim]Press Enter to go back...[/]");
                Console.ReadLine();
                wizardState = new WizardState();
                appState.CsvFilePath = null;
                break;
            }

            // Prompt for action (below layout)
            var validSongs = ValidateCsvStep.PromptForAction();

            if (validSongs.Count == 0)
            {
                // Go back
                wizardState = new WizardState();
                appState.CsvFilePath = null;
            }
            else
            {
                appState.ValidSongs = validSongs;
                wizardState.AdvanceToNextStep();
            }
            break;

        default:
            // Placeholder for remaining steps
            contentPanel = new Panel($"[dim]Step {wizardState.CurrentStep}[/]\n\nComing soon!")
                .Header($"[blue]{wizardState.CurrentStep}[/]")
                .Border(BoxBorder.Rounded);
            AppLayout.Render(stepsPanel, contentPanel);

            AnsiConsole.Markup("[dim]Press Enter to continue...[/]");
            Console.ReadLine();
            wizardState.AdvanceToNextStep();
            break;
    }
}

// Application state to store data between steps
class AppState
{
    public string? CsvFilePath { get; set; }
    public List<Song> ValidSongs { get; set; } = new();
}
