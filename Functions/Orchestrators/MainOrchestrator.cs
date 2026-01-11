using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using SpotifyUtilities.Functions.Activities;
using SpotifyUtility.Contracts.Activities;

namespace SpotifyUtilities.Functions.Orchestrators;

public static class MainOrchestrator
{
    [Function(nameof(MainOrchestrator))]
    public static async Task<FetchAlbumsByArtistResult> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(MainOrchestrator));

        logger.LogInformation("Saying hello.");

        // Replace name and input with values relevant for your Durable Functions Activity
        var output = await context.CallActivityAsync<FetchAlbumsByArtistResult>(
            nameof(FetchAlbumsByArtist.RunFetchAlbumsByArtist), 
            new FetchAlbumsByArtistInput(Guid.Empty.ToString(), 0)
        );

        // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        return output;
    }
}