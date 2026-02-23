using Newtonsoft.Json;
using SpotifyUtilities.Contracts;

namespace SpotifyUtilities.Services;

public class SpotifyAccessTokenService : ISpotifyAccessTokenService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SpotifyAccessTokenService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<SpotifyAccessToken> ExchangeAuthorizationCodeAsync(SpotifyAccessTokenRequest tokenRequest)
    {
        var client = _httpClientFactory.CreateClient();
        var tokenResponse = await client.PostAsync(SpotifyAccessTokenRequest.TokenEndpoint, tokenRequest.ToRequestBody());
        tokenResponse.EnsureSuccessStatusCode();

        var json = await tokenResponse.Content.ReadAsStringAsync();
        var accessTokenResponse = JsonConvert.DeserializeObject<SpotifyAccessTokenResponse>(json)
                                  ?? throw new Exception("Failed to deserialize Spotify access token response.");

        return new SpotifyAccessToken(
            accessTokenResponse.AccessToken,
            DateTime.UtcNow.AddSeconds(accessTokenResponse.ExpiresIn),
            accessTokenResponse.RefreshToken);
    }
}