using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SpotifyUtilities.Configuration;
using SpotifyUtilities.Contracts;
using SpotifyUtilities.Data;
using SpotifyUtilities.Utilities;

namespace SpotifyUtilities.Functions.Starters;

public class SpotifyLoginCallback
{
    private readonly ILoginAttemptRepository _loginAttemptRepository;
    private readonly SpotifyOptions _spotifyOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISpotifyAccessTokenRepository _spotifyAccessTokenRepository;

    public SpotifyLoginCallback(ILoginAttemptRepository loginAttemptRepository, IOptions<SpotifyOptions> spotifyOptions, IHttpClientFactory httpClientFactory, ISpotifyAccessTokenRepository spotifyAccessTokenRepository)
    {
        _loginAttemptRepository = loginAttemptRepository;
        _spotifyOptions = spotifyOptions.Value;
        _httpClientFactory = httpClientFactory;
        _spotifyAccessTokenRepository = spotifyAccessTokenRepository;
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

        // extract this to a separate service class 
        var client = _httpClientFactory.CreateClient();
        var tokenResponse = await client.PostAsync(SpotifyAccessTokenRequest.TokenEndpoint, tokenRequest.ToRequestBody());
        tokenResponse.EnsureSuccessStatusCode();
        var json = await tokenResponse.Content.ReadAsStringAsync();
        var accessTokenResponse = JsonConvert.DeserializeObject<SpotifyAccessTokenResponse>(json) ?? throw new Exception("Failed to deserialize Spotify access token response.");

        var accessToken = new SpotifyAccessToken(
            accessTokenResponse.AccessToken,
            DateTime.UtcNow.AddSeconds(accessTokenResponse.ExpiresIn),
            accessTokenResponse.RefreshToken);
        await _spotifyAccessTokenRepository.SaveAccessTokenAsync(accessToken);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Location", _spotifyOptions.FrontendRedirectUri);
        return response;
    }
}