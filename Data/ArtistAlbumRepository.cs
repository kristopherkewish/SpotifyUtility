using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using SpotifyUtilities.Configuration;
using SpotifyUtilities.Contracts;

namespace SpotifyUtilities.Data;

public class ArtistAlbumRepository : IArtistAlbumRepository
{
    private readonly Container _container;

    public ArtistAlbumRepository(IOptions<CosmosDbOptions> options)
    {
        var config = options.Value;
        var client = new CosmosClient(config.ConnectionString);
        var database = client.GetDatabase(config.DatabaseName);
        _container = database.GetContainer(config.ArtistAlbumsContainer);
    }

    public async Task<string> CreateArtistAlbumAsync(string jobId, string artistId, List<string> albumIds)
    {
        var artistAlbum = new ArtistAlbum(jobId, artistId, albumIds);
        await _container.CreateItemAsync(artistAlbum, new PartitionKey(jobId));
        return jobId;
    }
}