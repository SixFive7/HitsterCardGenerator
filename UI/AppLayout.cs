using Spectre.Console;
using Spectre.Console.Rendering;

namespace HitsterCardGenerator.UI;

public static class AppLayout
{
    public static void Render(IRenderable stepsPanel, IRenderable contentPanel)
    {
        // Clear the console
        AnsiConsole.Clear();

        // Create FIGlet header
        var header = new FigletText("Hitster Card Generator")
            .Color(Color.Blue);

        // Create the main layout with header and content area
        var layout = new Layout("Root")
            .SplitRows(
                new Layout("Header", header),
                new Layout("Main")
                    .SplitColumns(
                        new Layout("Steps", stepsPanel).Size(30),
                        new Layout("Content", contentPanel)
                    )
            );

        // Set header size
        layout["Header"].Size(10);

        // Render the layout
        AnsiConsole.Write(layout);
    }
}
