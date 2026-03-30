using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SpotifyUtilities.Contracts;
using SpotifyUtilities.Data;
using SpotifyUtility.Contracts.Activities;

namespace SpotifyUtilities.Functions.Activities;

public class PersistArtistAlbums(IArtistAlbumRepository artistAlbumRepository)
{
    private readonly IArtistAlbumRepository _artistAlbumRepository = artistAlbumRepository;

    [Function(nameof(PersistArtistAlbums))]
    public async Task RunPersistArtistAlbums([ActivityTrigger] PersistArtistAlbumsInput input, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("PersistArtistAlbums");
        logger.LogInformation("Persisting albums for job id {jobId}.", input.JobId);

        var albumEntries = input.Albums.Select(album => new AlbumEntry(album.AlbumId, int.Parse(album.ReleaseDate[..4]))).ToList();
        var artistAlbum = new ArtistAlbum(input.JobId, input.ArtistId, albumEntries);
        await _artistAlbumRepository.CreateArtistAlbumAsync(artistAlbum);
    }
}