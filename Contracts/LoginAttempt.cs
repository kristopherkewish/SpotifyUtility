using Newtonsoft.Json;

namespace SpotifyUtilities.Contracts;

/// <summary>
/// Represents a login attempt storing the PKCE code verifier for OAuth flow completion.
/// </summary>
/// <param name="Id">The unique identifier for the login attempt (Cosmos DB "id" field, also used as partition key).</param>
/// <param name="CodeVerifier">The PKCE code verifier to be used when exchanging the authorization code.</param>
public record LoginAttempt(
    [property: JsonProperty("id")] string Id,
    string CodeVerifier);
