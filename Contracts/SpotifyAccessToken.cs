using Newtonsoft.Json;

namespace SpotifyUtilities.Contracts;

/// <summary>
/// Represents a Spotify OAuth access token stored in Cosmos DB.
/// The access token itself serves as the unique identifier.
/// </summary>
/// <param name="AccessToken">The OAuth access token used to authenticate Spotify API requests. Also serves as the Cosmos DB document id.</param>
/// <param name="ExpiresAt">The UTC datetime when this access token expires.</param>
/// <param name="RefreshToken">The OAuth refresh token used to obtain a new access token when the current one expires.</param>
public record SpotifyAccessToken(
    [property: JsonProperty("id")] string AccessToken,
    DateTime ExpiresAt,
    string RefreshToken);