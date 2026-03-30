using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using SpotifyUtilities.Contracts;
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

        List<SpotifyAlbum> artistAlbums = [];
        var artistId = "6L7a6wPGpvLtTwOsMLnF1z";
        var total = 0;
        var jobId = context.NewGuid().ToString();

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

        // persist the album ids to the database
        var persistInput = new PersistArtistAlbumsInput(jobId, artistId, artistAlbums);
        await context.CallActivityAsync(nameof(PersistArtistAlbums), persistInput);

        logger.LogInformation("Fetched {count} albums for artist {artistId}.", artistAlbums.Count, artistId);
    }
}