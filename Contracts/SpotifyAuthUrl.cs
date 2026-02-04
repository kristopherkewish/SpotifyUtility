using Microsoft.AspNetCore.WebUtilities;

namespace SpotifyUtilities.Contracts;

/// <summary>
/// Represents a Spotify authorization URL with PKCE parameters.
/// </summary>
public class SpotifyAuthUrl(string clientId, string scope, string codeChallenge, string redirectUri, string state)
{
    private const string AuthorizeEndpoint = "https://accounts.spotify.com/authorize";

    public string ClientId { get; } = clientId;
    public string Scope { get; } = scope;
    public string CodeChallenge { get; } = codeChallenge;
    public string RedirectUri { get; } = redirectUri;
    public string State { get; } = state;

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
