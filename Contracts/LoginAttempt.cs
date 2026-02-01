using System.Text.Json.Serialization;

namespace SpotifyUtilities.Contracts;

/// <summary>
/// Represents a login attempt storing the PKCE code verifier for OAuth flow completion.
/// </summary>
/// <param name="LoginAttemptId">The unique identifier for the login attempt (also used as partition key).</param>
/// <param name="CodeVerifier">The PKCE code verifier to be used when exchanging the authorization code.</param>
public record LoginAttempt(string LoginAttemptId, string CodeVerifier)
{
    /// <summary>
    /// Document ID for Cosmos DB (serialized as lowercase "id").
    /// </summary>
    [JsonPropertyName("id")]
    public string Id => LoginAttemptId;
}
