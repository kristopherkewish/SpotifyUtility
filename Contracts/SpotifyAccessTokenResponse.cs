using Newtonsoft.Json;

namespace SpotifyUtilities.Contracts;

/// <summary>
/// Represents the JSON response from Spotify's OAuth token endpoint.
/// </summary>
/// <param name="AccessToken">The OAuth access token used to authenticate Spotify API requests.</param>
/// <param name="ExpiresIn">The number of seconds until the access token expires.</param>
/// <param name="RefreshToken">The OAuth refresh token used to obtain a new access token when the current one expires.</param>
public record SpotifyAccessTokenResponse(
    [property: JsonProperty("access_token")] string AccessToken,
    [property: JsonProperty("expires_in")] int ExpiresIn,
    [property: JsonProperty("refresh_token")] string RefreshToken);