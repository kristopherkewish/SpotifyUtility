using Microsoft.AspNetCore.WebUtilities;

namespace SpotifyUtilities.Contracts;

/// <summary>
/// Represents a Spotify authorization URL with PKCE parameters.
/// </summary>
public record SpotifyAuthUrl(
    string ClientId,
    string Scope,
    string CodeChallenge,
    string RedirectUri,
    string State
)
{
    public const string AuthorizeEndpoint = "https://accounts.spotify.com/authorize";

    public override string ToString()
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["response_type"] = "code",
            ["client_id"] = ClientId,
            ["scope"] = Scope,
            ["code_challenge_method"] = "S256",
            ["code_challenge"] = CodeChallenge,
            ["redirect_uri"] = RedirectUri,
            ["state"] = State
        };

        return QueryHelpers.AddQueryString(AuthorizeEndpoint, queryParams);
    }
}
