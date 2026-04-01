using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using SpotifyUtilities.Functions.Activities;
using SpotifyUtility.Contracts.Activities;

namespace SpotifyUtilities.Functions.Orchestrators;

public static class FetchAlbumTracksOrchestrator
{
    [Function(nameof(FetchAlbumTracksOrchestrator))]
    public static async Task<List<AlbumTracks>> RunFetchAlbumTracksOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context,
        ArtistAlbum album)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(FetchAlbumTracksOrchestrator));

        var initialInput = new FetchTracksByAlbumInput(album.AlbumId, 0);
        var initialOutput = await context.CallActivityAsync<FetchTracksByAlbumResult>(
            nameof(FetchTracksByAlbum),
            initialInput
        );

        var total = initialOutput.Total;
        var results = new List<AlbumTracks>();
        results.AddRange(initialOutput.AlbumTracks);

        var tasks = new List<Task<FetchTracksByAlbumResult>>();

        for(int offset = 10; offset <= total; offset += 10)
        {
            var input = new FetchTracksByAlbumInput(album.AlbumId, offset);

            tasks.Add(context.CallActivityAsync<FetchTracksByAlbumResult>(
                nameof(FetchTracksByAlbum),
                input
            ));
        }

        var taskResults = await Task.WhenAll(tasks);
        
        foreach(var taskResult in taskResults)
        {
            results.AddRange(taskResult.AlbumTracks);
        }

        return results;
    }
}