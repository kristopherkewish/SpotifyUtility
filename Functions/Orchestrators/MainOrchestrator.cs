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

        // Replace name and input with values relevant for your Durable Functions Activity
        // Test the activity function
        List<string> albumIds = [];
        var artistId = "6L7a6wPGpvLtTwOsMLnF1z";
        var total = 0;
        var jobId = new Guid().ToString();

        for (int offset = 0; offset <= total; offset += 10)
        {
            var input = new FetchAlbumsByArtistInput(artistId, offset);

            var output = await context.CallActivityAsync<FetchAlbumsByArtistResult>(
                nameof(FetchAlbumsByArtist),
                input
            );

            albumIds.AddRange(output.AlbumIds);
            total = output.Total;
        }

        logger.LogInformation("Fetched {count} albums for artist {artistId}.", albumIds.Count, artistId);
        albumIds.ForEach(albumId => logger.LogInformation("Album ID: {albumId}", albumId));
    }
}