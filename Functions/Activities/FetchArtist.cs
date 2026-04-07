using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SpotifyUtilities.Data;

namespace SpotifyUtilities.Functions.Activities;

public class FetchArtist(IHttpClientFactory httpClientFactory, ISpotifyAccessTokenRepository spotifyAccessTokenRepository)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ISpotifyAccessTokenRepository _spotifyAccessTokenRepository = spotifyAccessTokenRepository;

    [Function(nameof(FetchArtist))]
    public async Task<string> RunFetchArtist([ActivityTrigger] string id, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("FetchArtist");
        logger.LogInformation("Fetching artist with ID {id}.", id);

        string url = $"https://api.spotify.com/v1/artists/{id}";
        var accessToken = await _spotifyAccessTokenRepository.GetMostRecentAccessTokenAsync();

        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken?.AccessToken);
        var response = await httpClient.GetFromJsonAsync<SpotifyArtistResponseDTO>(url);

        return response?.Name ?? throw new InvalidOperationException($"Failed to fetch artist with ID {id}.");
    }

    private sealed record SpotifyArtistResponseDTO(string Name);
}