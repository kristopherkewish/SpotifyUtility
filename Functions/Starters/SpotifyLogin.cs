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

        string id = await _loginAttemptRepository.CreateLoginAttemptAsync(codeVerifier);

        var authUrl = new SpotifyAuthUrl(
            _spotifyOptions.ClientId,
            _spotifyOptions.Scope,
            codeChallenge,
            _spotifyOptions.RedirectUri,
            id);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync(authUrl.ToString());
        return response;
    }
}