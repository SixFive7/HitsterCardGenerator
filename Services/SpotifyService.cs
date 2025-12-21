using SpotifyAPI.Web;

namespace HitsterCardGenerator.Services;

public class SpotifyService
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private SpotifyClient? _client;

    public SpotifyService(string clientId, string clientSecret)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    /// <summary>
    /// Gets whether the service has successfully authenticated
    /// </summary>
    public bool IsAuthenticated => _client != null;

    /// <summary>
    /// Gets the authenticated Spotify client for API calls
    /// </summary>
    public SpotifyClient? Client => _client;

    /// <summary>
    /// Authenticates with Spotify using client credentials flow
    /// </summary>
    /// <returns>True if authentication succeeded, false otherwise</returns>
    public async Task<bool> AuthenticateAsync()
    {
        try
        {
            var config = SpotifyClientConfig.CreateDefault();
            var request = new ClientCredentialsRequest(_clientId, _clientSecret);
            var response = await new OAuthClient(config).RequestToken(request);

            _client = new SpotifyClient(config.WithToken(response.AccessToken));
            return true;
        }
        catch (APIException)
        {
            // Authentication failed (invalid credentials)
            _client = null;
            return false;
        }
        catch (Exception)
        {
            // Network or other error
            _client = null;
            return false;
        }
    }
}
