using Spectre.Console;
using Spectre.Console.Rendering;

namespace HitsterCardGenerator.UI;

public static class AppLayout
{
    /// <summary>
    /// Renders the app header and two-column layout (steps + content).
    /// Unlike Layout, this doesn't fill the screen - leaves room for prompts below.
    /// </summary>
    public static void Render(IRenderable stepsPanel, IRenderable contentPanel)
    {
        // Clear the console
        AnsiConsole.Clear();

        // Create FIGlet header
        var header = new FigletText("Hitster Card Generator")
            .Color(Color.Blue);

        AnsiConsole.Write(header);

        // Create side-by-side columns (doesn't fill screen like Layout does)
        var columns = new Columns(stepsPanel, contentPanel);
        columns.Expand = false;

        AnsiConsole.Write(columns);
        AnsiConsole.WriteLine();
    }
}
