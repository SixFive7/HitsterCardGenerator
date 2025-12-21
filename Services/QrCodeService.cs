using QRCoder;

namespace HitsterCardGenerator.Services;

/// <summary>
/// Service for generating QR codes linking to Spotify tracks
/// </summary>
public static class QrCodeService
{
    /// <summary>
    /// Generates a QR code PNG for a Spotify track
    /// </summary>
    /// <param name="spotifyTrackId">The Spotify track ID</param>
    /// <returns>PNG image data as byte array</returns>
    public static byte[] GenerateQrCode(string spotifyTrackId)
    {
        var url = $"https://open.spotify.com/track/{spotifyTrackId}";

        // 10 pixels per module gives ~330px image, good for 85mm card width
        return PngByteQRCodeHelper.GetQRCode(url, QRCodeGenerator.ECCLevel.Q, 10);
    }
}
