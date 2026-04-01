using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SpotifyUtilities.Data;
using SpotifyUtility.Contracts.Activities;

namespace SpotifyUtilities.Functions.Activities;

public class FetchTracksByAlbum(IHttpClientFactory httpClientFactory, ISpotifyAccessTokenRepository spotifyAccessTokenRepository)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ISpotifyAccessTokenRepository _spotifyAccessTokenRepository = spotifyAccessTokenRepository;

    [Function(nameof(FetchTracksByAlbum))]
    public async Task<FetchTracksByAlbumResult> RunFetchTracksByAlbum([ActivityTrigger] FetchTracksByAlbumInput input, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("FetchTracksByAlbum");
        logger.LogInformation("Fetching tracks for album {albumId}.", input.AlbumId);

        // build the fetch URL
        const int LIMIT = 10;
        const string MARKET = "AU";

        string baseUrl = $"https://api.spotify.com/v1/albums/{input.AlbumId}/tracks";

        var uriBuilder = new UriBuilder(baseUrl);
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["limit"] = LIMIT.ToString();
        parameters["market"] = MARKET;
        parameters["offset"] = input.Offset.ToString();
        uriBuilder.Query = parameters.ToString();
        var uri = uriBuilder.Uri;

        // get the access token
        var accessToken = await _spotifyAccessTokenRepository.GetMostRecentAccessTokenAsync();

        // make the fetch call
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken?.AccessToken);
        var response = await httpClient.GetFromJsonAsync<SpotifyAlbumTracksResponseDTO>(uri);

        // convert the result to a list of track IDs
        var albumTracks = response?.Items?.Select(track => new AlbumTracks(track.Id, track.Name)).ToList() ?? [];
        return new FetchTracksByAlbumResult(albumTracks, response?.Total ?? 0);
    }

    private sealed record SpotifyAlbumTracksResponseDTO(
        int Total,
        List<SpotifyTrackDTO> Items
    );

    private sealed record SpotifyTrackDTO(
        string Id,
        string Name
    );
}