using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SpotifyUtilities.Data;

namespace SpotifyUtilities.Functions.Activities;

public class CreatePlaylist(IHttpClientFactory httpClientFactory, ISpotifyAccessTokenRepository spotifyAccessTokenRepository)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ISpotifyAccessTokenRepository _spotifyAccessTokenRepository = spotifyAccessTokenRepository;

    [Function(nameof(CreatePlaylist))]
    public async Task<string> RunCreatePlaylist([ActivityTrigger] string name, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("CreatePlaylist");
        logger.LogInformation("Creating playlist with name {name}.", name);

        string url = $"https://api.spotify.com/v1/me/playlists";

        var accessToken = await _spotifyAccessTokenRepository.GetMostRecentAccessTokenAsync();

        // make the post call
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken?.AccessToken);
        var response = await httpClient.PostAsJsonAsync(url, new SpotifyCreatePlaylistRequestDTO(name));

        // convert the result to a list of track IDs
        var playlistId = await response.Content.ReadFromJsonAsync<SpotifyCreatePlaylistResponseDTO>() ?? throw new InvalidOperationException("Failed to create playlist.");
        return playlistId.Id;
    }
    
    private sealed record SpotifyCreatePlaylistResponseDTO(string Id);
    private sealed record SpotifyCreatePlaylistRequestDTO(string Name);
}

