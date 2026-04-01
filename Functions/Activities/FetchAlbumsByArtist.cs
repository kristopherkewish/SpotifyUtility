using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using SpotifyUtilities.Data;
using SpotifyUtility.Contracts.Activities;

namespace SpotifyUtilities.Functions.Activities;

public class FetchAlbumsByArtist(IHttpClientFactory httpClientFactory, ISpotifyAccessTokenRepository spotifyAccessTokenRepository)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ISpotifyAccessTokenRepository _spotifyAccessTokenRepository = spotifyAccessTokenRepository;

    [Function(nameof(FetchAlbumsByArtist))]
    public async Task<FetchAlbumsByArtistResult> RunFetchAlbumsByArtist([ActivityTrigger] FetchAlbumsByArtistInput input, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("FetchAlbumsByArtist");
        logger.LogInformation("Fetching albums for artist {name}.", input.ArtistId);

        // build the fetch URL
        const int LIMIT = 10;
        // the below causes an issue where the artist can appear on an album, and then later all of the tracks on that album are downloaded even if the artist is not on the track
        // causes the number of tracks to download to balloon significantly
        // const string INCLUDE_GROUPS = "album,single,appears_on,compilation";
        const string INCLUDE_GROUPS = "album,single";

        string baseUrl = $"https://api.spotify.com/v1/artists/{input.ArtistId}/albums";

        var uriBuilder = new UriBuilder(baseUrl);
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["limit"] = LIMIT.ToString();
        parameters["include_groups"] = INCLUDE_GROUPS;
        parameters["offset"] = input.Offset.ToString();
        uriBuilder.Query = parameters.ToString();
        var uri = uriBuilder.Uri;

        // get the access token
        var accessToken = await _spotifyAccessTokenRepository.GetMostRecentAccessTokenAsync();

        // make the fetch call
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken?.AccessToken);
        var response = await httpClient.GetFromJsonAsync<SpotifyArtistAlbumsResponseDTO>(uri);

        // convert the result to a list of album IDs
        var artistAlbums = response?.Items?.Select(album => new ArtistAlbum(album.Id, int.Parse(album.ReleaseDate[..4]))).ToList() ?? [];
        return new FetchAlbumsByArtistResult(artistAlbums, response?.Total ?? 0);
    }

    private sealed record SpotifyArtistAlbumsResponseDTO(
        string Id,
        int Total,
        List<SpotifyAlbumDTO> Items
    );

    private sealed record SpotifyAlbumDTO(
        string Id,
        [property: JsonPropertyName("release_date")] string ReleaseDate
    );
}