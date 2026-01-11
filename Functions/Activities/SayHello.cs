using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SpotifyUtilities.Functions.Activities;

public static class SayHello
{
    [Function(nameof(SayHello))]
    public static string RunSayHello([ActivityTrigger] string name, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SayHello");
        logger.LogInformation("Saying hello to {name}.", name);
        return $"Hello {name}!";
    }
}