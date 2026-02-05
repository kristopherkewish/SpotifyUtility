namespace SpotifyUtilities.Contracts;

public record SpotifyAccessTokenRequest(
    string GrantType,
    string Code,
    string RedirectUri,
    string ClientId,
    string CodeVerifier
)
{
    public const string TokenEndpoint = "https://accounts.spotify.com/api/token";

    public FormUrlEncodedContent ToRequestBody()
    {
        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = GrantType,
            ["code"] = Code,
            ["redirect_uri"] = RedirectUri,
            ["client_id"] = ClientId,
            ["code_verifier"] = CodeVerifier
        };

        return new FormUrlEncodedContent(formData);
    }
}
