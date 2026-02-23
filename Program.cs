using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotifyUtilities.Configuration;
using SpotifyUtilities.Data;
using SpotifyUtilities.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Configure strongly-typed options with validation
builder.Services.AddOptions<SpotifyOptions>()
    .Bind(builder.Configuration.GetSection(SpotifyOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<CosmosDbOptions>()
    .Bind(builder.Configuration.GetSection(CosmosDbOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Register HttpClientFactory for making HTTP requests
builder.Services.AddHttpClient();

builder.Services.AddSingleton<ILoginAttemptRepository, LoginAttemptRepository>();
builder.Services.AddSingleton<ISpotifyAccessTokenRepository, SpotifyAccessTokenRepository>();
builder.Services.AddSingleton<ISpotifyAccessTokenService, SpotifyAccessTokenService>();

builder.Build().Run();
