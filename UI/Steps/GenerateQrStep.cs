using Spectre.Console;
using Spectre.Console.Rendering;
using HitsterCardGenerator.Models;
using HitsterCardGenerator.Services;

namespace HitsterCardGenerator.UI.Steps;

public static class GenerateQrStep
{
    /// <summary>
    /// Gets the progress panel showing current QR generation state
    /// </summary>
    public static IRenderable GetProgressPanel(int current, int total, string currentSong)
    {
        var progress = (double)current / total;
        var percentage = (int)(progress * 100);
        var barWidth = 30;
        var filledWidth = (int)(barWidth * progress);
        var progressBar = new string('█', filledWidth) + new string('░', barWidth - filledWidth);

        var content = new Rows(
            new Markup($"[bold]Generating QR codes for Spotify tracks...[/]\n"),
            new Markup($"[dim]Progress:[/] [{(percentage == 100 ? "green" : "yellow")}]{progressBar}[/] {percentage}%"),
            new Markup($"[dim]Song {current} of {total}[/]\n"),
            new Markup($"[bold]Current:[/] {Markup.Escape(currentSong)}")
        );

        return new Panel(content)
            .Header("[blue]Step 5: Generate QR Codes[/]")
            .Border(BoxBorder.Rounded);
    }

    /// <summary>
    /// Generates QR codes for all songs with Spotify track IDs
    /// </summary>
    public static List<Song> GenerateQrCodes(
        List<Song> songs,
        Action<int, string> onProgress)
    {
        var result = new List<Song>();

        for (int i = 0; i < songs.Count; i++)
        {
            var song = songs[i];
            onProgress(i + 1, $"{song.Artist} - {song.Title}");

            if (!string.IsNullOrEmpty(song.SpotifyTrackId))
            {
                var qrData = QrCodeService.GenerateQrCode(song.SpotifyTrackId);
                result.Add(song with { QrCodeData = qrData });
            }
            else
            {
                result.Add(song);
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the summary panel showing completion message
    /// </summary>
    public static IRenderable GetSummaryPanel(int total)
    {
        var content = new Rows(
            new Markup($"[bold green]QR Code Generation Complete![/]\n"),
            new Markup($"Generated [green]{total}[/] QR codes linking to Spotify tracks.\n"),
            new Markup("[dim]Each QR code links to: https://open.spotify.com/track/{{id}}[/]\n"),
            new Markup("\n[yellow]Press Enter to continue to card design...[/]")
        );

        return new Panel(content)
            .Header("[blue]Step 5: Generate QR Codes - Complete[/]")
            .Border(BoxBorder.Rounded);
    }
}
