using Spectre.Console;
using Spectre.Console.Rendering;
using HitsterCardGenerator.Services;

namespace HitsterCardGenerator.UI.Steps;

public static class SpotifyCredentialsStep
{
    /// <summary>
    /// Returns the content panel with instructions AND prompt text included.
    /// </summary>
    public static IRenderable GetContentPanel(string? errorMessage = null)
    {
        var content =
            "[bold]Spotify API Authentication[/]\n\n" +
            "To search for tracks on Spotify, you need API credentials.\n\n" +
            "[yellow]How to get your credentials:[/]\n" +
            "1. Go to [link=https://developer.spotify.com/dashboard]developer.spotify.com/dashboard[/]\n" +
            "2. Log in with your Spotify account\n" +
            "3. Click \"Create App\"\n" +
            "4. Fill in any app name and description\n" +
            "5. Redirect URI: enter [cyan]https://localhost[/] and click Add\n" +
            "6. Check [cyan]Web API[/] under \"Which API/SDKs are you planning to use?\"\n" +
            "7. Accept terms and click Save\n" +
            "8. Copy the [cyan]Client ID[/] and [cyan]Client Secret[/] from Settings\n\n";

        if (!string.IsNullOrEmpty(errorMessage))
        {
            content += $"[red]{errorMessage}[/]\n\n";
        }

        content += "[yellow]Enter your Spotify Client ID below:[/]";

        return new Panel(content)
            .Header("[blue]Step 3: Spotify Credentials[/]")
            .Border(BoxBorder.Rounded);
    }

    /// <summary>
    /// Returns a panel for the second prompt (Client Secret).
    /// </summary>
    public static IRenderable GetSecretPromptPanel()
    {
        var content = "[yellow]Enter your Spotify Client Secret below:[/]";

        return new Panel(content)
            .Header("[blue]Step 3: Spotify Credentials[/]")
            .Border(BoxBorder.Rounded);
    }

    /// <summary>
    /// Prompts for Client ID and Client Secret.
    /// </summary>
    public static (string clientId, string clientSecret) PromptForCredentials()
    {
        var clientId = AnsiConsole.Prompt(
            new TextPrompt<string>("> ")
                .PromptStyle("green")
                .AllowEmpty());

        // Clear and show next prompt
        AnsiConsole.Clear();

        var clientSecret = AnsiConsole.Prompt(
            new TextPrompt<string>("> ")
                .PromptStyle("green")
                .Secret()
                .AllowEmpty());

        return (clientId?.Trim() ?? "", clientSecret?.Trim() ?? "");
    }

    /// <summary>
    /// Validates credentials against Spotify API.
    /// </summary>
    public static async Task<string?> ValidateCredentialsAsync(string clientId, string clientSecret)
    {
        if (string.IsNullOrWhiteSpace(clientId))
        {
            return "Client ID cannot be empty.";
        }

        if (string.IsNullOrWhiteSpace(clientSecret))
        {
            return "Client Secret cannot be empty.";
        }

        var service = new SpotifyService(clientId, clientSecret);
        var success = await service.AuthenticateAsync();

        if (!success)
        {
            return "Authentication failed. Please check your credentials and try again.";
        }

        return null; // Success
    }
}
