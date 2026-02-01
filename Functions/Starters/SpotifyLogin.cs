using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpotifyUtilities.Configuration;
using SpotifyUtilities.Data;
using SpotifyUtilities.Utilities;

namespace SpotifyUtilities.Functions.Starters;

public class SpotifyLogin
{
    private readonly ILoginAttemptRepository _loginAttemptRepository;
    private readonly SpotifyOptions _spotifyOptions;

    public SpotifyLogin(ILoginAttemptRepository loginAttemptRepository, IOptions<SpotifyOptions> spotifyOptions)
    {
        _loginAttemptRepository = loginAttemptRepository;
        _spotifyOptions = spotifyOptions.Value;
    }

    [Function("SpotifyLogin")]
    public async Task<HttpResponseData> RunSpotifyLogin(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SpotifyLogin");

        string codeVerifier = PkceGenerator.GenerateCodeVerifier();
        string codeChallenge = PkceGenerator.GenerateCodeChallenge(codeVerifier);

        string loginAttemptId = await _loginAttemptRepository.CreateLoginAttemptAsync(codeVerifier);

        var authUrl = BuildAuthUrl(codeChallenge, loginAttemptId);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync(authUrl);
        return response;
    }

    private string BuildAuthUrl(string codeChallenge, string loginAttemptId)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["response_type"] = "code",
            ["client_id"] = _spotifyOptions.ClientId,
            ["scope"] = _spotifyOptions.Scope,
            ["code_challenge_method"] = "S256",
            ["code_challenge"] = codeChallenge,
            ["redirect_uri"] = _spotifyOptions.RedirectUri,
            ["state"] = loginAttemptId
        };

        return QueryHelpers.AddQueryString("https://accounts.spotify.com/authorize", queryParams);
    }
}