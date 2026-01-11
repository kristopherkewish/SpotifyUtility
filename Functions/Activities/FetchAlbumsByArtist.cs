using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SpotifyUtility.Contracts.Activities;

namespace SpotifyUtilities.Functions.Activities;

public static class FetchAlbumsByArtist
{
    [Function(nameof(FetchAlbumsByArtist))]
    public static FetchAlbumsByArtistResult RunFetchAlbumsByArtist([ActivityTrigger] FetchAlbumsByArtistInput input, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("FetchAlbumsByArtist");
        logger.LogInformation("Fetching albums for artist {name}.", input.ArtistId);
        return new FetchAlbumsByArtistResult(new string[] { new Guid().ToString(), new Guid().ToString() });
    }
}