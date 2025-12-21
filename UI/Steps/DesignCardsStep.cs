using Spectre.Console;
using Spectre.Console.Rendering;
using HitsterCardGenerator.Models;

namespace HitsterCardGenerator.UI.Steps;

public static class DesignCardsStep
{
    /// <summary>
    /// Gets the progress panel showing card design progress
    /// </summary>
    public static IRenderable GetProgressPanel(int current, int total)
    {
        var progress = (double)current / total;
        var percentage = (int)(progress * 100);
        var barWidth = 30;
        var filledWidth = (int)(barWidth * progress);
        var progressBar = new string('█', filledWidth) + new string('░', barWidth - filledWidth);

        var content = new Rows(
            new Markup($"[bold]Preparing card designs...[/]\n"),
            new Markup($"[dim]Progress:[/] [{(percentage == 100 ? "green" : "yellow")}]{progressBar}[/] {percentage}%"),
            new Markup($"[dim]Card {current} of {total}[/]")
        );

        return new Panel(content)
            .Header("[blue]Step 7: Design Cards[/]")
            .Border(BoxBorder.Rounded);
    }

    /// <summary>
    /// Prepares CardData from songs
    /// </summary>
    public static List<CardData> PrepareCards(List<Song> songs, bool useColors, Action<int> onProgress)
    {
        var result = new List<CardData>();

        for (int i = 0; i < songs.Count; i++)
        {
            onProgress(i + 1);
            result.Add(CardData.FromSong(songs[i], useColors));
        }

        return result;
    }

    /// <summary>
    /// Gets the summary panel after card design
    /// </summary>
    public static IRenderable GetSummaryPanel(int cardCount)
    {
        var content = new Rows(
            new Markup($"[bold green]Card Designs Complete![/]\n"),
            new Markup($"Prepared [green]{cardCount}[/] cards for export.\n"),
            new Markup("\n[yellow]Press Enter to continue to PDF export...[/]")
        );

        return new Panel(content)
            .Header("[blue]Step 7: Design Cards - Complete[/]")
            .Border(BoxBorder.Rounded);
    }
}
