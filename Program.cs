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

        case Step.SpotifyAuth:
            // Loop until valid credentials are provided
            string? authError = null;
            while (true)
            {
                stepsPanel = StepMenu.Render(wizardState);
                contentPanel = SpotifyCredentialsStep.GetContentPanel(authError);
                AppLayout.Render(stepsPanel, contentPanel);

                // Prompt for Client ID
                var clientId = AnsiConsole.Prompt(
                    new TextPrompt<string>("> ")
                        .PromptStyle("green")
                        .AllowEmpty());

                if (string.IsNullOrWhiteSpace(clientId))
                {
                    authError = "Client ID cannot be empty.";
                    continue;
                }

                // Show secret prompt
                stepsPanel = StepMenu.Render(wizardState);
                contentPanel = SpotifyCredentialsStep.GetSecretPromptPanel();
                AppLayout.Render(stepsPanel, contentPanel);

                // Prompt for Client Secret (masked)
                var clientSecret = AnsiConsole.Prompt(
                    new TextPrompt<string>("> ")
                        .PromptStyle("green")
                        .Secret()
                        .AllowEmpty());

                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    authError = "Client Secret cannot be empty.";
                    continue;
                }

                // Validate credentials
                AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .Start("[yellow]Authenticating with Spotify...[/]", ctx =>
                    {
                        var service = new SpotifyService(clientId.Trim(), clientSecret.Trim());
                        var success = service.AuthenticateAsync().GetAwaiter().GetResult();

                        if (success)
                        {
                            appState.SpotifyService = service;
                            authError = null;
                        }
                        else
                        {
                            authError = "Authentication failed. Please check your credentials.";
                        }
                    });

                if (authError == null)
                {
                    break; // Success
                }
            }
            wizardState.AdvanceToNextStep();
            break;

        case Step.SpotifySearch:
            // Process songs with Spotify search
            stepsPanel = StepMenu.Render(wizardState);
            var initialProgressPanel = SpotifySearchStep.GetProgressPanel(0, appState.ValidSongs.Count, "Starting...", "Initializing...");
            AppLayout.Render(stepsPanel, initialProgressPanel);

            // Run search asynchronously
            var searchResults = SpotifySearchStep.ProcessSongsAsync(
                appState.ValidSongs,
                appState.SpotifyService!,
                (steps, content) => AppLayout.Render(steps, content),
                stepsPanel
            ).GetAwaiter().GetResult();

            // Update songs with Spotify track IDs
            appState.ValidSongs = searchResults
                .Where(r => r.Status == SearchStatus.Found && r.Match != null)
                .Select(r => r.Song with { SpotifyTrackId = r.Match!.TrackId })
                .ToList();

            // Show summary
            stepsPanel = StepMenu.Render(wizardState);
            var summaryPanel = SpotifySearchStep.GetSummaryPanel(searchResults);
            AppLayout.Render(stepsPanel, summaryPanel);

            // Prompt for action
            var foundCount = searchResults.Count(r => r.Status == SearchStatus.Found);
            if (SpotifySearchStep.PromptForAction(foundCount))
            {
                wizardState.AdvanceToNextStep();
            }
            else
            {
                // Go back to CSV import
                wizardState = new WizardState();
                appState.CsvFilePath = null;
                appState.ValidSongs = new List<Song>();
                appState.SpotifyService = null;
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
    public SpotifyService? SpotifyService { get; set; }
}
