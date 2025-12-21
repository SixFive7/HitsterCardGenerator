using Spectre.Console;
using Spectre.Console.Rendering;
using HitsterCardGenerator.Services;

namespace HitsterCardGenerator.UI.Steps;

public static class ExportPdfStep
{
    /// <summary>
    /// Gets the content panel explaining export options
    /// </summary>
    public static IRenderable GetContentPanel(int cardCount)
    {
        var pageCount = PdfExporter.GetPageCount(cardCount);

        var content = new Rows(
            new Markup("[bold]Export to PDF[/]\n"),
            new Markup($"Ready to generate PDF with [green]{cardCount}[/] cards.\n"),
            new Markup($"This will create [cyan]{pageCount}[/] pages ({pageCount / 2} sheets, front + back).\n\n"),
            new Markup("[dim]Layout: 2 columns × 5 rows = 10 cards per sheet[/]\n"),
            new Markup("[dim]Card size: 85mm × 55mm (credit card size)[/]\n\n"),
            new Markup("[yellow]Enter output file path:[/]")
        );

        return new Panel(content)
            .Header("[blue]Step 8: Export PDF[/]")
            .Border(BoxBorder.Rounded);
    }

    /// <summary>
    /// Prompts user for output file path
    /// </summary>
    public static string PromptForOutputPath()
    {
        var path = AnsiConsole.Prompt(
            new TextPrompt<string>("> ")
                .PromptStyle("green")
                .DefaultValue("hitster-cards.pdf"));

        // Ensure .pdf extension
        if (!path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            path += ".pdf";
        }

        return path;
    }

    /// <summary>
    /// Gets the exporting panel shown during PDF generation
    /// </summary>
    public static IRenderable GetExportingPanel(string outputPath)
    {
        var content = new Rows(
            new Markup("[bold yellow]Generating PDF...[/]\n"),
            new Markup($"[dim]Output:[/] {Markup.Escape(outputPath)}")
        );

        return new Panel(content)
            .Header("[blue]Step 8: Export PDF[/]")
            .Border(BoxBorder.Rounded);
    }

    /// <summary>
    /// Gets the completion panel with export statistics
    /// </summary>
    public static IRenderable GetCompletionPanel(string outputPath, int cardCount, int pageCount)
    {
        var table = new Table()
            .Border(TableBorder.Simple)
            .BorderColor(Color.Grey)
            .AddColumn("Property")
            .AddColumn("Value");

        table.AddRow("Output file", Markup.Escape(outputPath));
        table.AddRow("Cards generated", cardCount.ToString());
        table.AddRow("Total pages", pageCount.ToString());
        table.AddRow("Sheets (front+back)", (pageCount / 2).ToString());

        var content = new Rows(
            new Markup("[bold green]PDF Export Complete![/]\n"),
            table,
            new Markup("\n[bold cyan]Printing Instructions:[/]"),
            new Markup("[dim]1. Print double-sided (duplex)[/]"),
            new Markup("[dim]2. Flip on short edge[/]"),
            new Markup("[dim]3. Cut along the gray lines[/]\n"),
            new Markup("\n[bold green]✓ Hitster Card Generator Complete![/]\n"),
            new Markup("[dim]Press Enter to exit...[/]")
        );

        return new Panel(content)
            .Header("[blue]Step 8: Export PDF - Complete[/]")
            .Border(BoxBorder.Rounded);
    }
}
