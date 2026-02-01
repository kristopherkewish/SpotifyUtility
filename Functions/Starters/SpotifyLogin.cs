using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using SpotifyUtilities.Functions.Orchestrators;
using System.Security.Cryptography;
using System.Text;

namespace SpotifyUtilities.Functions.Starters;

public static class SpotifyLogin
{

    [Function("SpotifyLogin")]
    public static async Task<HttpResponseData> RunSpotifyLogin(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SpotifyLogin");

        ValidateEnvironmentVariables();
        string codeVerifier = GenerateCodeVerifier(64);
        string loginAttemptId = await StoreCodeVerifier(codeVerifier);
        string codeChallenge = GenerateCodeChallenge(codeVerifier);
        var authUrl = BuildAuthUrl(codeChallenge, loginAttemptId);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync(authUrl);
        return response;
    }

    private static string GenerateCodeVerifier(int length)
    {
        const string possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var randomBytes = RandomNumberGenerator.GetBytes(length);
        return new string([.. randomBytes.Select(x => possible[x % possible.Length])]);
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        byte[] challengeBytes = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
        return Convert.ToBase64String(challengeBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static async Task<string> StoreCodeVerifier(string codeVerifier)
    {
        string loginAttemptId = Guid.NewGuid().ToString();
        var connectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString");

        var client = new CosmosClient(connectionString);
        var database = client.GetDatabase("SpotifyUtility");
        var container = database.GetContainer("LoginAttempts");

        var loginAttempt = new LoginAttempt(loginAttemptId, codeVerifier);
        await container.CreateItemAsync(loginAttempt, new PartitionKey(loginAttemptId));

        return loginAttemptId;
    }

    private static string BuildAuthUrl(string codeChallenge, string loginAttemptId)
    {
        var clientId = Environment.GetEnvironmentVariable("SpotifyClientId");
        var redirectUri = Environment.GetEnvironmentVariable("SpotifyRedirectUri");

        var scope = "playlist-modify-public";

        var queryParams = new Dictionary<string, string?>
        {
            ["response_type"] = "code",
            ["client_id"] = clientId,
            ["scope"] = scope,
            ["code_challenge_method"] = "S256",
            ["code_challenge"] = codeChallenge,
            ["redirect_uri"] = redirectUri,
            ["state"] = loginAttemptId
        };

        string authUrl = QueryHelpers.AddQueryString("https://accounts.spotify.com/authorize", queryParams);
        return authUrl;
    }

    private static void ValidateEnvironmentVariables()
    {
        List<string> requiredVariables = ["SpotifyClientId", "SpotifyRedirectUri", "CosmosDBConnectionString"];
        requiredVariables.ForEach(ValidateEnvironmentVariable);
    }
    private static void ValidateEnvironmentVariable(string name)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
        {
            throw new InvalidOperationException($"{name} is not set in environment variables.");
        }
    }

    public record LoginAttempt(string LoginAttemptId, string CodeVerifier);
}