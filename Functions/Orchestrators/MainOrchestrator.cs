using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using SpotifyUtilities.Functions.Activities;
using SpotifyUtility.Contracts.Activities;

namespace SpotifyUtilities.Functions.Orchestrators;

public static class MainOrchestrator
{
    [Function(nameof(MainOrchestrator))]
    public static async Task RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(MainOrchestrator));

        logger.LogInformation("Starting orchestrator.");

        List<ArtistAlbum> artistAlbums = [];
        var artistId = "6L7a6wPGpvLtTwOsMLnF1z";
        var total = 0;
        var jobId = new Guid().ToString();
        var yearStart = 2010;
        var yearEnd = 2011;

        // initial call to get total number of albums for the artist
        var initialInput = new FetchAlbumsByArtistInput(artistId, 0);
        var initialOutput = await context.CallActivityAsync<FetchAlbumsByArtistResult>(
            nameof(FetchAlbumsByArtist),
            initialInput
        );
        artistAlbums.AddRange(initialOutput.ArtistAlbums);
        total = initialOutput.Total;

        // parallelize the rest of the calls
        var tasks = new List<Task<FetchAlbumsByArtistResult>>();

        for (int offset = 10; offset <= total; offset += 10)
        {
            var input = new FetchAlbumsByArtistInput(artistId, offset);

            tasks.Add(context.CallActivityAsync<FetchAlbumsByArtistResult>(
                nameof(FetchAlbumsByArtist),
                input
            ));
        }

        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            artistAlbums.AddRange(result.ArtistAlbums);
        }

        var filteredAlbums = artistAlbums.Where(album => album.ReleaseYear >= yearStart && album.ReleaseYear <= yearEnd).ToList();

        List<AlbumTracks> tracksResults = [];

        // Parallelize calls to fetch album tracks in batches of 10, prevent hitting Spotify API limits
        foreach(var albumChunk in filteredAlbums.Chunk(10))
        {
            var albumChunkTasks = new List<Task<List<AlbumTracks>>>();

            foreach(var album in albumChunk)
            {
                albumChunkTasks.Add(context.CallSubOrchestratorAsync<List<AlbumTracks>>(
                    nameof(FetchAlbumTracksOrchestrator),
                    album
                ));
            }

            var albumChunkResults = await Task.WhenAll(albumChunkTasks);
            
            foreach (var albumTracks in albumChunkResults)
            {
                tracksResults.AddRange(albumTracks);
            }
        }

        logger.LogInformation("Fetched {count} albums for artist {artistId}.", filteredAlbums.Count, artistId);
        logger.LogInformation("Fetched {count} tracks for artist {artistId}.", tracksResults.Count, artistId);
    }
}