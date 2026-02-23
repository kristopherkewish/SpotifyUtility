using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpotifyUtilities.Configuration;
using SpotifyUtilities.Contracts;
using SpotifyUtilities.Data;
using SpotifyUtilities.Services;
using SpotifyUtilities.Utilities;

namespace SpotifyUtilities.Functions.Starters;

public class SpotifyLoginCallback
{
    private readonly ILoginAttemptRepository _loginAttemptRepository;
    private readonly SpotifyOptions _spotifyOptions;
    private readonly ISpotifyAccessTokenRepository _spotifyAccessTokenRepository;
    private readonly ISpotifyAccessTokenService _spotifyAccessTokenService;

    public SpotifyLoginCallback(ILoginAttemptRepository loginAttemptRepository, IOptions<SpotifyOptions> spotifyOptions, ISpotifyAccessTokenRepository spotifyAccessTokenRepository, ISpotifyAccessTokenService spotifyAccessTokenService)
    {
        _loginAttemptRepository = loginAttemptRepository;
        _spotifyOptions = spotifyOptions.Value;
        _spotifyAccessTokenRepository = spotifyAccessTokenRepository;
        _spotifyAccessTokenService = spotifyAccessTokenService;
    }

    [Function("SpotifyLoginCallback")]
    public async Task<HttpResponseData> RunSpotifyLoginCallback(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SpotifyLoginCallback");

        string? code = req.Query["code"] ?? throw new ArgumentException("Missing required query parameter 'code'.");
        string? loginAttemptId = req.Query["state"] ?? throw new ArgumentException("Missing required query parameter 'state'.");

        var loginAttempt = await _loginAttemptRepository.GetLoginAttemptAsync(loginAttemptId) ?? throw new ArgumentException("Invalid login attempt ID.");

        var tokenRequest = new SpotifyAccessTokenRequest(
            "authorization_code",
            code,
            _spotifyOptions.RedirectUri,
            _spotifyOptions.ClientId,
            loginAttempt.CodeVerifier);

        var accessToken = await _spotifyAccessTokenService.ExchangeAuthorizationCodeAsync(tokenRequest);
        await _spotifyAccessTokenRepository.SaveAccessTokenAsync(accessToken);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Location", _spotifyOptions.FrontendRedirectUri);
        return response;
    }
}